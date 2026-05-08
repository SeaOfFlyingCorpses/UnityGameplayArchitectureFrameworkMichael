using UnityEngine;
using Framework.Systems.Damage;
using Framework.Core;
using Gameplay.Systems.Health;

namespace Gameplay.Systems.Damage
{
    // =========================================
    // WeakpointComponent
    // Attach to a child GameObject (e.g. head,
    // eye, glowing core) to make it a weakpoint.
    //
    // When hit, multiplies incoming damage by
    // the weakpoint multiplier and optionally
    // restricts to specific damage types.
    //
    // Requires a Collider on the same GameObject.
    // The parent must have a HealthComponent.
    //
    // Setup:
    //   1. Add empty child to enemy (e.g. "Head")
    //   2. Add SphereCollider to child
    //   3. Add WeakpointComponent to child
    //   4. Set Multiplier (e.g. 2 = double damage)
    //   5. HealthComponent is found automatically
    //      by searching up the hierarchy
    // =========================================
    public class WeakpointComponent : MonoBehaviour
    {
        [Header("Weakpoint Settings")]
        [Tooltip("Damage multiplier when this point is hit")]
        public float multiplier = 2f;

        [Tooltip("Only these damage types trigger weakpoint bonus. " +
                 "Empty = all types trigger it.")]
        public DamageType[] vulnerableTo;

        [Tooltip("Visual/audio effect key when weakpoint is hit")]
        public string hitEffectId = "";

        private HealthComponent _health;

        private void Awake()
        {
            // Find HealthComponent anywhere in parent hierarchy
            _health = GetComponentInParent<HealthComponent>();

            if (_health == null)
                Debug.LogWarning(
                    $"[WeakpointComponent] {gameObject.name} — " +
                    "No HealthComponent found in parent hierarchy.");
        }

        // =========================================
        // Called by projectile or melee hit
        // =========================================
        public void Hit(DamageInfo info)
        {
            if (_health == null) return;

            // Check if damage type triggers weakpoint
            if (vulnerableTo != null && vulnerableTo.Length > 0)
            {
                bool vulnerable = false;
                foreach (var type in vulnerableTo)
                {
                    if (type == info.Type)
                    {
                        vulnerable = true;
                        break;
                    }
                }
                if (!vulnerable)
                {
                    // Deal normal damage, no bonus
                    _health.Damage(info.Amount, info.HitPoint);
                    return;
                }
            }

            // Apply weakpoint multiplier
            int amplified = Mathf.RoundToInt(info.Amount * multiplier);

            var amplifiedInfo = new DamageInfo(
                amplified,
                info.Type,
                true, // treat weakpoint hits as critical
                info.HitPoint,
                info.Source);

            _health.Damage(amplifiedInfo.Amount, amplifiedInfo.HitPoint);

            // Publish hit effect if configured
            if (!string.IsNullOrEmpty(hitEffectId))
                Framework.Events.EventBus.Publish(
                    new Framework.Events.Events.Gameplay.ProjectileHitEvent(
                        info.Source?.gameObject,
                        gameObject,
                        info.HitPoint,
                        -transform.forward,
                        amplified));
        }
    }
}
