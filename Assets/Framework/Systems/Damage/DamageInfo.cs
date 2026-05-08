using UnityEngine;

namespace Framework.Systems.Damage
{
    // =========================================
    // DamageInfo
    // All information about a single damage event.
    // Passed to IHealth.Damage() — richer than
    // just an int amount.
    //
    // Usage:
    //   health.Damage(new DamageInfo
    //   {
    //       Amount     = 25,
    //       Type       = DamageType.Fire,
    //       IsCritical = true,
    //       HitPoint   = transform.position,
    //       Source     = attackerTransform
    //   });
    // =========================================
    public struct DamageInfo
    {
        public int        Amount;
        public DamageType Type;
        public bool       IsCritical;
        public Vector3    HitPoint;
        public Transform  Source;

        // =========================================
        // Convenience constructors
        // =========================================
        public DamageInfo(int amount)
        {
            Amount     = amount;
            Type       = DamageType.Physical;
            IsCritical = false;
            HitPoint   = Vector3.zero;
            Source     = null;
        }

        public DamageInfo(
            int        amount,
            DamageType type,
            bool       isCritical = false,
            Vector3    hitPoint   = default,
            Transform  source     = null)
        {
            Amount     = amount;
            Type       = type;
            IsCritical = isCritical;
            HitPoint   = hitPoint;
            Source     = source;
        }
    }
}
