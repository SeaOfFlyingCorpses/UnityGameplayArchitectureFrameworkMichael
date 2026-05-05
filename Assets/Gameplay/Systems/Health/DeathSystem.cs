using System;
using UnityEngine;
using Framework.Core;

namespace Gameplay.Systems.Health
{
    public class DeathSystem : MonoBehaviour
    {
        [SerializeField] private float  destroyDelay = 2f;

        [Tooltip("If set — returns to this pool key instead of destroying." +
                 " Leave empty to destroy normally.")]
        [SerializeField] private string poolKey = "";

        public event Action OnDeath;

        public void Register(HealthComponent health, Rigidbody rb)
        {
            if (health == null)
                return;

            health.OnDeath += () => HandleDeath(health, rb);
        }

        private void HandleDeath(HealthComponent health, Rigidbody rb)
        {
            if (rb != null)
            {
                rb.linearVelocity  = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic     = true;
            }

            var col = health.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            OnDeath?.Invoke();

            // =========================================
            // POOL OR DESTROY
            // If a pool key is set and a PoolRegistry
            // exists — return to pool after delay.
            // Otherwise fall back to Destroy.
            // =========================================
            var registry = ServiceLocator.Get<PoolRegistry>();

            if (!string.IsNullOrEmpty(poolKey) &&
                registry != null               &&
                registry.HasPool(poolKey))
            {
                registry.ReturnDelayed(poolKey, health.gameObject, destroyDelay);
            }
            else
            {
                Destroy(health.gameObject, destroyDelay);
            }
        }
    }
}