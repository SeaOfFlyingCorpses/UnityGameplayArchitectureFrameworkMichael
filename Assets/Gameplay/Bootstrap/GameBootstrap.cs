using UnityEngine;
using Framework.Core;

namespace Gameplay.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [System.Serializable]
        public class PoolEntry
        {
            [Tooltip("Unique key — must match Pool Key in DeathSystem and any spawner")]
            public string     key;

            [Tooltip("The prefab to pool")]
            public GameObject prefab;

            [Tooltip("How many instances to pre-warm at startup")]
            public int        initialSize = 10;
        }

        [Header("Object Pools")]
        public PoolEntry[] pools;

        // =========================================
        // Use Start() not Awake() so PoolRegistry
        // has time to register itself in its Awake()
        // =========================================
        private void Start()
        {
            RegisterPools();
        }

        private void RegisterPools()
        {
            var registry = ServiceLocator.Get<PoolRegistry>();

            if (registry == null)
            {
                Debug.LogWarning("[GameBootstrap] No PoolRegistry found.");
                return;
            }

            if (pools == null || pools.Length == 0)
                return;

            foreach (var entry in pools)
            {
                if (string.IsNullOrEmpty(entry.key))
                {
                    Debug.LogWarning("[GameBootstrap] Pool entry has no key — skipping.");
                    continue;
                }

                if (entry.prefab == null)
                {
                    Debug.LogWarning($"[GameBootstrap] Pool '{entry.key}' has no prefab — skipping.");
                    continue;
                }

                registry.CreatePool(entry.key, entry.prefab, entry.initialSize);
            }
        }
    }
}