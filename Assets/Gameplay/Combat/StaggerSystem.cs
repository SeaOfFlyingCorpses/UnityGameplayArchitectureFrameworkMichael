using Gameplay.Systems.Health;
using UnityEngine;

namespace Gameplay.Combat
{
    public class StaggerSystem : MonoBehaviour
    {
        [SerializeField] private float force = 8f;

        public void Register(HealthComponent health, Rigidbody rb)
        {
            health.OnHit += (hitPoint) =>
            {
                Vector3 dir = (health.transform.position - hitPoint).normalized;

                Debug.DrawRay(health.transform.position, dir * 2f, Color.red, 0.5f);
                Debug.Log("HIT → STAGGER TRIGGERED");
                // UPDATED API
                rb.linearVelocity = Vector3.zero;

                rb.AddForce(dir * force, ForceMode.Impulse);
            };
        }
    }
}