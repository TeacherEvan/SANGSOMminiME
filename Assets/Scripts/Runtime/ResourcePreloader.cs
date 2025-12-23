using UnityEngine;
using System.Collections;
using System;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Preloads and caches frequently accessed Resources to reduce load times.
    /// Implements lazy loading patterns and memory-efficient caching strategies.
    /// Optimized for Unity 2022.3 LTS with async operations and progress tracking.
    /// </summary>
    public class ResourcePreloader : MonoBehaviour
    {
        [Header("Preload Settings")]
        [SerializeField] private bool preloadOnAwake = true;
        [SerializeField] private bool showLoadingProgress = true;
        [SerializeField] private float preloadDelay = 0.5f;
        
        [Header("Resource Paths")]
        [SerializeField] private string[] outfitPaths = { "Outfits/default", "Outfits/casual", "Outfits/formal" };
        [SerializeField] private string[] accessoryPaths = { "Accessories/none", "Accessories/hat1", "Accessories/glasses" };
        [SerializeField] private string[] uiSpritePaths = { };
        
        [Header("Preload Progress")]
        [SerializeField] private bool enableDetailedLogging = false;
        
        // Delegate caching/loading to ResourceCache to avoid redundant cache implementations.
        private ResourceCache cacheBackend;
        
        // Preload state
        private bool isPreloading = false;
        private bool preloadComplete = false;
        private float preloadProgress = 0f;
        
        // Singleton instance
        private static ResourcePreloader instance;
        public static ResourcePreloader Instance => instance;
        
        // Events
        public event Action OnPreloadStarted;
        public event Action<float> OnPreloadProgress;
        public event Action OnPreloadComplete;
        
        // Public accessors
        public bool IsPreloading => isPreloading;
        public bool PreloadComplete => preloadComplete;
        public float PreloadProgress => preloadProgress;
        
        private void Awake()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                EnsureCacheBackend();
                
                if (preloadOnAwake)
                {
                    StartCoroutine(PreloadResourcesWithDelay(preloadDelay));
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void EnsureCacheBackend()
        {
            if (cacheBackend != null) return;

            cacheBackend = ResourceCache.Instance;
            if (cacheBackend != null) return;

            var cacheObject = new GameObject("ResourceCache");
            cacheBackend = cacheObject.AddComponent<ResourceCache>();
        }
        
        /// <summary>
        /// Starts preloading resources after a short delay.
        /// Delay allows critical systems to initialize first.
        /// </summary>
        private IEnumerator PreloadResourcesWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartPreload();
        }
        
        /// <summary>
        /// Starts the resource preloading process.
        /// Can be called manually if preloadOnAwake is false.
        /// </summary>
        public void StartPreload()
        {
            if (isPreloading || preloadComplete)
            {
                Debug.LogWarning("[ResourcePreloader] Preload already started or completed.");
                return;
            }
            
            StartCoroutine(PreloadResourcesCoroutine());
        }
        
        /// <summary>
        /// Preloads all configured resources asynchronously with progress tracking.
        /// Implements chunked loading to avoid frame spikes.
        /// </summary>
        private IEnumerator PreloadResourcesCoroutine()
        {
            isPreloading = true;
            preloadProgress = 0f;
            OnPreloadStarted?.Invoke();

            EnsureCacheBackend();
            
            LogInfo("Starting resource preload...");
            
            // Calculate total items to load
            int totalItems = outfitPaths.Length + accessoryPaths.Length + uiSpritePaths.Length;
            int loadedItems = 0;
            
            if (totalItems == 0)
            {
                LogInfo("No resources configured for preload.");
                CompletePreload();
                yield break;
            }
            
            // Preload outfits
            foreach (var path in outfitPaths)
            {
                yield return StartCoroutine(LoadResourceAsync<GameObject>(path));
                loadedItems++;
                UpdateProgress(loadedItems, totalItems);
                
                // Yield every few items to prevent frame spikes
                if (loadedItems % 3 == 0)
                    yield return null;
            }
            
            // Preload accessories
            foreach (var path in accessoryPaths)
            {
                yield return StartCoroutine(LoadResourceAsync<GameObject>(path));
                loadedItems++;
                UpdateProgress(loadedItems, totalItems);
                
                if (loadedItems % 3 == 0)
                    yield return null;
            }
            
            // Preload UI sprites
            foreach (var path in uiSpritePaths)
            {
                yield return StartCoroutine(LoadResourceAsync<Sprite>(path));
                loadedItems++;
                UpdateProgress(loadedItems, totalItems);
                
                if (loadedItems % 3 == 0)
                    yield return null;
            }
            
            CompletePreload();
        }
        
        /// <summary>
        /// Loads a resource asynchronously and caches it.
        /// Uses ResourceCache for non-blocking loads and consistent eviction behavior.
        /// </summary>
        private IEnumerator LoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(path)) yield break;

            EnsureCacheBackend();
            if (cacheBackend == null) yield break;

            // Skip if already cached
            if (cacheBackend.IsResourceCached(path))
            {
                LogInfo($"Resource already cached: {path}", true);
                yield break;
            }

            bool completed = false;
            T loaded = null;

            cacheBackend.LoadResourceAsync<T>(path, (resource) =>
            {
                loaded = resource;
                completed = true;
            });

            while (!completed)
            {
                yield return null;
            }

            if (loaded != null)
            {
                LogInfo($"Preloaded resource: {path}", true);
            }
            else
            {
                Debug.LogWarning($"[ResourcePreloader] Failed to load resource: {path}");
            }
        }
        
        /// <summary>
        /// Updates preload progress and notifies listeners.
        /// </summary>
        private void UpdateProgress(int loaded, int total)
        {
            preloadProgress = (float)loaded / total;
            OnPreloadProgress?.Invoke(preloadProgress);
            
            if (showLoadingProgress)
            {
                LogInfo($"Preload progress: {loaded}/{total} ({preloadProgress:P0})", true);
            }
        }
        
        /// <summary>
        /// Marks preload as complete and notifies listeners.
        /// </summary>
        private void CompletePreload()
        {
            isPreloading = false;
            preloadComplete = true;
            preloadProgress = 1f;
            
            OnPreloadComplete?.Invoke();
            EnsureCacheBackend();
            if (cacheBackend != null)
            {
                LogInfo($"Resource preload complete. {cacheBackend.GetCacheStatistics()}");
            }
            else
            {
                LogInfo("Resource preload complete.");
            }
        }
        
        /// <summary>
        /// Gets a cached resource by path. Returns null if not cached.
        /// Automatically loads if not found and caching is enabled.
        /// </summary>
        /// <typeparam name="T">Type of resource to retrieve</typeparam>
        /// <param name="path">Resource path</param>
        /// <returns>Cached resource or null</returns>
        public T GetCachedResource<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            EnsureCacheBackend();
            if (cacheBackend == null) return null;

            // Delegate to ResourceCache (loads on demand if not cached)
            return cacheBackend.LoadResource<T>(path);
        }
        
        /// <summary>
        /// Checks if a resource is already cached.
        /// </summary>
        public bool IsResourceCached(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            EnsureCacheBackend();
            if (cacheBackend == null) return false;

            return cacheBackend.IsResourceCached(path);
        }
        
        /// <summary>
        /// Clears the resource cache to free memory.
        /// Call during scene transitions or when memory is constrained.
        /// </summary>
        public void ClearCache()
        {
            EnsureCacheBackend();
            if (cacheBackend == null)
            {
                LogInfo("Resource cache cleared. Freed 0 cached resources.");
                return;
            }

            cacheBackend.ClearAll();
            
            // TODO: [OPTIMIZATION] Selective cache clearing based on usage patterns (Implement when cache >50 items)
            // Performance thresholds:
            // - Implement if: resourceCache.Count > 50 OR memory pressure detected (>80% heap usage)
            // - Track access frequency: Dictionary<string, int> accessCount (increment on GetCachedResource)
            // - Keep top 20 most accessed resources in cache
            // - Clear resources not accessed in last 5 minutes
            // - Profile memory savings: Expect 30-50% reduction for typical usage
            // Implementation: ~2-3 hours, add LRU cache or access count tracking
            
            LogInfo("Resource cache cleared.");
        }
        
        /// <summary>
        /// Gets cache statistics for debugging and optimization.
        /// </summary>
        public void LogCacheStatistics()
        {
            EnsureCacheBackend();
            Debug.Log($"[ResourcePreloader] Cache Statistics:");
            Debug.Log($"  Preload Complete: {preloadComplete}");
            if (cacheBackend == null)
            {
                Debug.Log($"  Backend: (none)");
                return;
            }

            Debug.Log($"  Backend Stats: {cacheBackend.GetCacheStatistics()}");
        }
        
        private void LogInfo(string message, bool detailedOnly = false)
        {
            if (!detailedOnly || enableDetailedLogging)
            {
                Debug.Log($"[ResourcePreloader] {message}");
            }
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            ClearCache();
        }
    }
}
