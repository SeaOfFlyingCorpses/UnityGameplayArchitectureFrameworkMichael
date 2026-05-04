using UnityEngine;

namespace Gameplay.Systems.Health
{
    public class DeathSystem : MonoBehaviour
    {
        public void Register(HealthComponent health, Rigidbody rb)
        {
            health.OnDeath += () =>
            {
                // stop physics
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }

                // disable AI (if present)
                var ai = health.GetComponent<Gameplay.AI.AIController>();
                if (ai != null)
                    ai.enabled = false;

                // disable collider
                var col = health.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;

                // optional: destroy after delay
                Destroy(health.gameObject, 2f);
            };
        }
    }
}