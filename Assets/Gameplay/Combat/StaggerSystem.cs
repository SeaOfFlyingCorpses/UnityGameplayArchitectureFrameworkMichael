using Gameplay.Systems.Health;
using UnityEngine;

namespace Gameplay.Combat
{
    public class StaggerSystem : MonoBehaviour
    {
        [SerializeField] private float force = 4f;

        public void Register(HealthComponent health, Rigidbody rb)
        {
            if (health == null || rb == null)
                return;

            health.OnHit += (hitPoint) =>
            {
                // Guard — do nothing if already dead / kinematic
                if (rb == null || rb.isKinematic)
                    return;

                Vector3 dir = (health.transform.position - hitPoint).normalized;

                Debug.DrawRay(health.transform.position, dir * 2f, Color.red, 0.5f);

                rb.linearVelocity = Vector3.zero;
                rb.AddForce(dir * force, ForceMode.Impulse);
            };
        }
    }
}