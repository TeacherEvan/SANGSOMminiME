using UnityEngine;
using System.Collections.Generic;
using System;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Generic object pooling system for performance optimization.
    /// Reduces memory allocation and GC pressure by reusing game objects.
    /// Implements modern Unity 2022.3 LTS best practices for pooling.
    /// </summary>
    /// <typeparam name="T">Type of component to pool</typeparam>
    public class ObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform poolParent;
        private readonly Queue<T> availableObjects;
        private readonly HashSet<T> activeObjects;
        private readonly int initialCapacity;
        private readonly int maxCapacity;
        private readonly bool expandable;

        /// <summary>
        /// Gets the count of available (inactive) objects in the pool.
        /// </summary>
        public int AvailableCount => availableObjects.Count;

        /// <summary>
        /// Gets the count of active (in-use) objects from the pool.
        /// </summary>
        public int ActiveCount => activeObjects.Count;

        /// <summary>
        /// Gets the total capacity of the pool.
        /// </summary>
        public int TotalCapacity => AvailableCount + ActiveCount;

        /// <summary>
        /// Creates a new object pool with specified configuration.
        /// </summary>
        /// <param name="prefab">Prefab to instantiate</param>
        /// <param name="initialCapacity">Number of objects to pre-instantiate</param>
        /// <param name="maxCapacity">Maximum pool size (0 for unlimited)</param>
        /// <param name="expandable">Whether pool can grow beyond initial capacity</param>
        /// <param name="poolParent">Parent transform for pooled objects (null for scene root)</param>
        public ObjectPool(T prefab, int initialCapacity = 10, int maxCapacity = 0, bool expandable = true, Transform poolParent = null)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab), "Prefab cannot be null");

            if (initialCapacity < 0)
                throw new ArgumentException("Initial capacity cannot be negative", nameof(initialCapacity));

            if (maxCapacity < 0)
                throw new ArgumentException("Max capacity cannot be negative", nameof(maxCapacity));

            if (maxCapacity > 0 && initialCapacity > maxCapacity)
                throw new ArgumentException("Initial capacity cannot exceed max capacity");

            this.prefab = prefab;
            this.initialCapacity = initialCapacity;
            this.maxCapacity = maxCapacity;
            this.expandable = expandable;
            this.poolParent = poolParent;
            this.availableObjects = new Queue<T>(initialCapacity);
            this.activeObjects = new HashSet<T>();

            PrewarmPool();
        }

        /// <summary>
        /// Pre-instantiates objects to avoid runtime allocation spikes.
        /// </summary>
        private void PrewarmPool()
        {
            for (int i = 0; i < initialCapacity; i++)
            {
                T obj = CreateNewObject();
                obj.gameObject.SetActive(false);
                availableObjects.Enqueue(obj);
            }
        }

        /// <summary>
        /// Creates a new instance of the pooled object.
        /// </summary>
        private T CreateNewObject()
        {
            T obj = UnityEngine.Object.Instantiate(prefab, poolParent);
            obj.gameObject.name = $"{prefab.name}_Pooled_{TotalCapacity}";
            return obj;
        }

        /// <summary>
        /// Gets an object from the pool. Creates new if needed and allowed.
        /// </summary>
        /// <returns>Available object from pool or null if pool exhausted</returns>
        public T Get()
        {
            T obj = null;

            // Try to get from available objects
            if (availableObjects.Count > 0)
            {
                obj = availableObjects.Dequeue();
            }
            // Check if we can expand
            else if (expandable && (maxCapacity == 0 || TotalCapacity < maxCapacity))
            {
                obj = CreateNewObject();
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] Pool exhausted for {typeof(T).Name}. Consider increasing pool size.");
                return null;
            }

            // Activate and track
            obj.gameObject.SetActive(true);
            activeObjects.Add(obj);

            return obj;
        }

        /// <summary>
        /// Gets an object and positions it at specified location.
        /// </summary>
        public T Get(Vector3 position, Quaternion rotation)
        {
            T obj = Get();
            if (obj != null)
            {
                obj.transform.SetPositionAndRotation(position, rotation);
            }
            return obj;
        }

        /// <summary>
        /// Returns an object to the pool for reuse.
        /// </summary>
        /// <param name="obj">Object to return</param>
        public void Return(T obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("[ObjectPool] Attempted to return null object to pool.");
                return;
            }

            if (!activeObjects.Remove(obj))
            {
                Debug.LogWarning($"[ObjectPool] Object {obj.name} was not tracked as active. Possible duplicate return.");
                return;
            }

            // Reset and deactivate
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(poolParent);
            availableObjects.Enqueue(obj);
        }

        /// <summary>
        /// Returns all active objects to the pool.
        /// </summary>
        public void ReturnAll()
        {
            // Create temporary list to avoid modifying collection during iteration
            var tempList = new List<T>(activeObjects);
            foreach (var obj in tempList)
            {
                Return(obj);
            }
        }

        /// <summary>
        /// Destroys all pooled objects and clears the pool.
        /// Call during cleanup or scene transitions.
        /// </summary>
        public void Clear()
        {
            // Destroy available objects
            while (availableObjects.Count > 0)
            {
                var obj = availableObjects.Dequeue();
                if (obj != null)
                    UnityEngine.Object.Destroy(obj.gameObject);
            }

            // Destroy active objects
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                    UnityEngine.Object.Destroy(obj.gameObject);
            }

            activeObjects.Clear();
        }
    }

    /// <summary>
    /// Manages multiple object pools with centralized access.
    /// Implements singleton pattern for global pool management.
    /// TODO: [OPTIMIZATION] Consider implementing pool warming based on scene analytics
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        private static ObjectPoolManager instance;
        public static ObjectPoolManager Instance => instance;

        private Dictionary<Type, object> pools = new Dictionary<Type, object>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Gets or creates a pool for the specified prefab type.
        /// </summary>
        public ObjectPool<T> GetOrCreatePool<T>(T prefab, int initialCapacity = 10, int maxCapacity = 0, bool expandable = true) where T : Component
        {
            Type type = typeof(T);

            if (pools.TryGetValue(type, out object existingPool))
            {
                return existingPool as ObjectPool<T>;
            }

            var newPool = new ObjectPool<T>(prefab, initialCapacity, maxCapacity, expandable, transform);
            pools[type] = newPool;

            Debug.Log($"[ObjectPoolManager] Created new pool for {type.Name} with capacity {initialCapacity}");
            return newPool;
        }

        /// <summary>
        /// Clears all pools. Call during scene cleanup.
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                var clearMethod = pool.GetType().GetMethod("Clear");
                clearMethod?.Invoke(pool, null);
            }
            pools.Clear();
        }

        private void OnDestroy()
        {
            ClearAllPools();
        }
    }
}
