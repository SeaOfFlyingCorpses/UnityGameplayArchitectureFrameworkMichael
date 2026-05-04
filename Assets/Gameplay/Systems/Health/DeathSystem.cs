using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    public class DeathSystem : MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 2f;

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
                // Zero velocity BEFORE setting kinematic —
                // setting linearVelocity on a kinematic body causes a warning
                rb.linearVelocity  = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic     = true;
            }

            var col = health.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            OnDeath?.Invoke();

            Destroy(health.gameObject, destroyDelay);
        }
    }
}