using System;
using System.Collections.Generic;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Optimized resource cache with lazy loading and memory management.
    /// Implements object pooling pattern for frequently accessed resources.
    /// 
    /// TODO: [OPTIMIZATION] Consider migrating to Addressables when:
    ///       - Total Resources assets exceed 100MB (check Build Report)
    ///       - More than 50 unique resources loaded per scene
    ///       - Memory profiling shows excessive heap fragmentation
    ///       - Need for runtime content updates or DLC support
    ///       - Targeting mobile with <2GB RAM
    /// </summary>
    public class ResourceCache : MonoBehaviour
    {
        [Header("Cache Configuration")]
        [SerializeField] private int maxCacheSize = 50;
        [SerializeField] private bool enableAutomaticCleanup = true;
        [SerializeField] private float cleanupInterval = 60f;

        // Singleton pattern
        private static ResourceCache instance;
        public static ResourceCache Instance => instance;

        // Cache storage with LRU tracking
        private readonly Dictionary<string, CachedResource> resourceCache = new Dictionary<string, CachedResource>();
        private readonly Queue<string> accessOrder = new Queue<string>();

        // Statistics for monitoring
        private int cacheHits;
        private int cacheMisses;
        
        private float lastCleanupTime;

        /// <summary>
        /// Cached resource wrapper with metadata.
        /// </summary>
        private class CachedResource
        {
            public UnityEngine.Object Resource { get; set; }
            public float LastAccessTime { get; set; }
            public int AccessCount { get; set; }
            public int SizeEstimate { get; set; }

            public CachedResource(UnityEngine.Object resource)
            {
                Resource = resource;
                LastAccessTime = Time.time;
                AccessCount = 1;
                SizeEstimate = EstimateResourceSize(resource);
            }

            /// <summary>
            /// Estimates memory footprint of resource for cache management.
            /// </summary>
            private static int EstimateResourceSize(UnityEngine.Object resource)
            {
                // Simplified size estimation
                return resource switch
                {
                    Texture2D texture => texture.width * texture.height * 4, // Rough RGBA estimate
                    AudioClip audio => (int)(audio.samples * audio.channels * 2), // 16-bit audio
                    Mesh mesh => mesh.vertexCount * 32, // Approximate vertex data
                    Material => 1024, // Small material data
                    GameObject => 2048, // Prefab with components
                    _ => 512 // Default small size
                };
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                lastCleanupTime = Time.time;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Loads resource with caching and lazy loading.
        /// Generic version for type safety.
        /// </summary>
        /// <typeparam name="T">Type of resource to load</typeparam>
        /// <param name="resourcePath">Path relative to Resources folder</param>
        /// <returns>Loaded resource or null if not found</returns>
        public T LoadResource<T>(string resourcePath) where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                Debug.LogWarning("[ResourceCache] Cannot load resource: Path is null or empty.");
                return null;
            }

            // Check cache first
            if (resourceCache.TryGetValue(resourcePath, out var cached))
            {
                UpdateCacheAccess(resourcePath, cached);
                cacheHits++;
                return cached.Resource as T;
            }

            // Cache miss - load from Resources
            cacheMisses++;
            var resource = Resources.Load<T>(resourcePath);

            if (resource == null)
            {
                Debug.LogWarning($"[ResourceCache] Resource not found at path: {resourcePath}");
                return null;
            }

            // Add to cache with size management
            AddToCache(resourcePath, resource);

            return resource;
        }

        /// <summary>
        /// Async resource loading with callback pattern.
        /// Provides better user experience for large assets.
        /// </summary>
        /// <typeparam name="T">Type of resource to load</typeparam>
        /// <param name="resourcePath">Path relative to Resources folder</param>
        /// <param name="onLoaded">Callback invoked when resource is loaded</param>
        public void LoadResourceAsync<T>(string resourcePath, Action<T> onLoaded) where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                Debug.LogWarning("[ResourceCache] Cannot load resource: Path is null or empty.");
                onLoaded?.Invoke(null);
                return;
            }

            // Check cache first
            if (resourceCache.TryGetValue(resourcePath, out var cached))
            {
                UpdateCacheAccess(resourcePath, cached);
                cacheHits++;
                onLoaded?.Invoke(cached.Resource as T);
                return;
            }

            // Async load from Resources
            cacheMisses++;
            var request = Resources.LoadAsync<T>(resourcePath);
            
            request.completed += (operation) =>
            {
                var resource = request.asset as T;
                
                if (resource != null)
                {
                    AddToCache(resourcePath, resource);
                }
                else
                {
                    Debug.LogWarning($"[ResourceCache] Async resource not found at path: {resourcePath}");
                }

                onLoaded?.Invoke(resource);
            };
        }

        /// <summary>
        /// Preloads multiple resources for smooth gameplay.
        /// Reduces runtime loading stutters.
        /// </summary>
        /// <typeparam name="T">Type of resources to preload</typeparam>
        /// <param name="resourcePaths">Array of resource paths</param>
        /// <param name="onComplete">Optional callback when all resources loaded</param>
        public void PreloadResources<T>(string[] resourcePaths, Action onComplete = null) where T : UnityEngine.Object
        {
            if (resourcePaths == null || resourcePaths.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }

            int loadedCount = 0;
            int totalCount = resourcePaths.Length;

            foreach (var path in resourcePaths)
            {
                LoadResourceAsync<T>(path, (resource) =>
                {
                    loadedCount++;
                    
                    if (loadedCount >= totalCount)
                    {
                        Debug.Log($"[ResourceCache] Preloaded {totalCount} resources of type {typeof(T).Name}");
                        onComplete?.Invoke();
                    }
                });
            }
        }

        /// <summary>
        /// Updates cache access metadata for LRU eviction.
        /// </summary>
        private void UpdateCacheAccess(string path, CachedResource cached)
        {
            cached.LastAccessTime = Time.time;
            cached.AccessCount++;
        }

        /// <summary>
        /// Adds resource to cache with size management and LRU eviction.
        /// </summary>
        private void AddToCache(string path, UnityEngine.Object resource)
        {
            // Enforce cache size limit
            while (resourceCache.Count >= maxCacheSize)
            {
                EvictLeastRecentlyUsed();
            }

            resourceCache[path] = new CachedResource(resource);
            accessOrder.Enqueue(path);
        }

        /// <summary>
        /// Evicts least recently used resource from cache.
        /// </summary>
        private void EvictLeastRecentlyUsed()
        {
            if (accessOrder.Count == 0) return;

            string pathToEvict = accessOrder.Dequeue();
            
            if (resourceCache.ContainsKey(pathToEvict))
            {
                resourceCache.Remove(pathToEvict);
                Debug.Log($"[ResourceCache] Evicted resource: {pathToEvict}");
            }
        }

        /// <summary>
        /// Manually clears specific resource from cache.
        /// Useful for memory management of large assets.
        /// </summary>
        /// <param name="resourcePath">Path of resource to clear</param>
        public void ClearResource(string resourcePath)
        {
            if (resourceCache.Remove(resourcePath))
            {
                Debug.Log($"[ResourceCache] Cleared resource: {resourcePath}");
            }
        }

        /// <summary>
        /// Clears all cached resources.
        /// Call this during scene transitions or memory warnings.
        /// </summary>
        public void ClearAll()
        {
            int count = resourceCache.Count;
            resourceCache.Clear();
            accessOrder.Clear();
            Debug.Log($"[ResourceCache] Cleared all cached resources ({count} items)");
        }

        /// <summary>
        /// Returns cache statistics for monitoring and optimization.
        /// </summary>
        public string GetCacheStatistics()
        {
            float hitRate = cacheHits + cacheMisses > 0
                ? (float)cacheHits / (cacheHits + cacheMisses) * 100f
                : 0f;

            return $"Cache Stats - Size: {resourceCache.Count}/{maxCacheSize}, " +
                   $"Hits: {cacheHits}, Misses: {cacheMisses}, " +
                   $"Hit Rate: {hitRate:F1}%";
        }

        private void Update()
        {
            // Periodic automatic cleanup
            if (enableAutomaticCleanup && Time.time - lastCleanupTime >= cleanupInterval)
            {
                PerformAutomaticCleanup();
                lastCleanupTime = Time.time;
            }
        }

        /// <summary>
        /// Removes old, infrequently accessed resources automatically.
        /// </summary>
        private void PerformAutomaticCleanup()
        {
            var itemsToRemove = new List<string>();
            float currentTime = Time.time;

            // Remove resources not accessed in last 5 minutes with low access count
            foreach (var kvp in resourceCache)
            {
                if (currentTime - kvp.Value.LastAccessTime > 300f && kvp.Value.AccessCount < 3)
                {
                    itemsToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in itemsToRemove)
            {
                resourceCache.Remove(key);
            }

            if (itemsToRemove.Count > 0)
            {
                Debug.Log($"[ResourceCache] Automatic cleanup removed {itemsToRemove.Count} old resources");
            }
        }

        private void OnDestroy()
        {
            ClearAll();
        }

#if UNITY_EDITOR
        // Editor-only debugging
        [ContextMenu("Print Cache Statistics")]
        private void PrintCacheStatistics()
        {
            Debug.Log(GetCacheStatistics());
            
            foreach (var kvp in resourceCache)
            {
                var cached = kvp.Value;
                Debug.Log($"  - {kvp.Key}: Accessed {cached.AccessCount} times, " +
                         $"Last: {Time.time - cached.LastAccessTime:F1}s ago, " +
                         $"Size: ~{cached.SizeEstimate / 1024f:F1}KB");
            }
        }

        [ContextMenu("Clear Cache")]
        private void EditorClearCache()
        {
            ClearAll();
        }
#endif
    }
}
