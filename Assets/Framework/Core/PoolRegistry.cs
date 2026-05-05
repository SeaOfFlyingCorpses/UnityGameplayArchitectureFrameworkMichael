using System.Collections.Generic;
using UnityEngine;
using Framework.Core;

namespace Framework.Core
{
    // =========================================
    // PoolRegistry
    // Central registry for all object pools.
    // Registered as a ServiceLocator service.
    //
    // Place on the _GameSystems GameObject.
    //
    // Usage:
    //   // Register a pool
    //   ServiceLocator.Get<PoolRegistry>()
    //       .CreatePool("Enemy", enemyPrefab, 10);
    //
    //   // Get an instance
    //   var obj = ServiceLocator.Get<PoolRegistry>()
    //       .Get("Enemy", position, rotation);
    //
    //   // Return an instance
    //   ServiceLocator.Get<PoolRegistry>()
    //       .Return("Enemy", obj);
    // =========================================
    public class PoolRegistry : MonoBehaviour
    {
        private readonly Dictionary<string, GameObjectPool> _pools = new();

        private void Awake()
        {
            ServiceLocator.Register<PoolRegistry>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<PoolRegistry>();
        }

        // =========================================
        // CREATE POOL
        // =========================================
        public void CreatePool(string key, GameObject prefab, int initialSize)
        {
            if (_pools.ContainsKey(key))
            {
                Debug.LogWarning($"[PoolRegistry] Pool '{key}' already exists.");
                return;
            }

            // Create a container transform to keep hierarchy clean
            var container     = new GameObject($"Pool_{key}");
            container.transform.SetParent(transform);

            _pools[key] = new GameObjectPool(prefab, initialSize, container.transform);

            Debug.Log($"[PoolRegistry] Created pool '{key}' with {initialSize} instances.");
        }

        // =========================================
        // GET — returns an active instance
        // =========================================
        public GameObject Get(string key, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(key, out var pool))
            {
                Debug.LogWarning($"[PoolRegistry] Pool '{key}' not found.");
                return null;
            }

            return pool.Get(position, rotation);
        }

        // =========================================
        // RETURN — returns instance to pool
        // =========================================
        public void Return(string key, GameObject obj)
        {
            if (!_pools.TryGetValue(key, out var pool))
            {
                Debug.LogWarning($"[PoolRegistry] Pool '{key}' not found. Destroying instead.");
                Destroy(obj);
                return;
            }

            pool.Return(obj);
        }

        // =========================================
        // RETURN DELAYED
        // =========================================
        public void ReturnDelayed(string key, GameObject obj, float delay)
        {
            if (!_pools.TryGetValue(key, out var pool))
            {
                Debug.LogWarning($"[PoolRegistry] Pool '{key}' not found. Destroying instead.");
                Destroy(obj, delay);
                return;
            }

            pool.ReturnDelayed(obj, delay, this);
        }

        // =========================================
        // HAS POOL
        // =========================================
        public bool HasPool(string key)
        {
            return _pools.ContainsKey(key);
        }
    }
}
