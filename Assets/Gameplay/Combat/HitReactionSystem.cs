using Gameplay.Systems.Health;
using UnityEngine;

namespace Gameplay.Combat
{
    public class HitReactionSystem : MonoBehaviour
    {
        public void Register(HealthComponent health, Rigidbody rb)
        {
            health.OnHit += (hitPoint) =>
            {
                Vector3 dir = (health.transform.position - hitPoint).normalized;

                rb.AddForce(dir * 5f, ForceMode.Impulse);
            };
        }
    }
}