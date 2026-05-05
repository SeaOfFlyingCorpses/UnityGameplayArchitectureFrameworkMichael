using UnityEngine;
using Framework.Core;

namespace Gameplay.Bootstrap
{
    // =========================================
    // GameBootstrap
    // Registers all object pools at game start.
    // Place on the _GameSystems GameObject.
    //
    // Add a new pool entry in the Inspector for
    // each prefab you want pooled.
    // The Key must match what DeathSystem and
    // any spawner uses to get/return objects.
    //
    // Example keys:
    //   "Enemy"      — standard enemy
    //   "EliteEnemy" — elite variant
    //   "Bullet"     — projectile
    //   "Explosion"  — VFX
    //   "Loot"       — dropped item
    // =========================================
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

        private void Awake()
        {
            RegisterPools();
        }

        private void RegisterPools()
        {
            var registry = ServiceLocator.Get<PoolRegistry>();

            if (registry == null)
            {
                Debug.LogWarning("[GameBootstrap] No PoolRegistry found. " +
                                 "Make sure PoolRegistry is on the same GameObject " +
                                 "and its Awake runs before GameBootstrap.");
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
