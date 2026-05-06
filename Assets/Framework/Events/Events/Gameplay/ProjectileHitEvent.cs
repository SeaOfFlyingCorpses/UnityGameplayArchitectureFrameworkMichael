using UnityEngine;

namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // ProjectileHitEvent
    // Fired via EventBus when a projectile hits.
    // Subscribe to react without coupling to
    // the projectile system directly.
    //
    // Usage:
    //   EventBus.Subscribe<ProjectileHitEvent>(OnHit);
    //
    //   private void OnHit(ProjectileHitEvent e)
    //   {
    //       // spawn VFX at e.HitPoint
    //       // play audio
    //       // update score
    //   }
    // =========================================
    public struct ProjectileHitEvent
    {
        public GameObject Source;      // projectile that hit
        public GameObject Target;      // what was hit
        public Vector3    HitPoint;    // world position of hit
        public Vector3    HitNormal;   // surface normal
        public int        Damage;      // damage dealt

        public ProjectileHitEvent(
            GameObject source,
            GameObject target,
            Vector3    hitPoint,
            Vector3    hitNormal,
            int        damage)
        {
            Source    = source;
            Target    = target;
            HitPoint  = hitPoint;
            HitNormal = hitNormal;
            Damage    = damage;
        }
    }
}
