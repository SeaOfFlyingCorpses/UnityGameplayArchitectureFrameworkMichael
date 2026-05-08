using UnityEngine;

namespace Framework.Systems.Damage
{
    // =========================================
    // CriticalHitSystem
    // Pure C# — no MonoBehaviour.
    // Calculates critical hits and multipliers.
    //
    // Usage:
    //   var result = CriticalHitSystem.Calculate(
    //       baseDamage: 20,
    //       critChance: 0.25f,
    //       critMultiplier: 2f);
    //
    //   health.Damage(new DamageInfo(
    //       result.FinalDamage,
    //       DamageType.Physical,
    //       result.IsCritical));
    // =========================================
    public static class CriticalHitSystem
    {
        public struct CritResult
        {
            public int  FinalDamage;
            public bool IsCritical;
        }

        // =========================================
        // Calculate
        // critChance    0-1 (0.25 = 25% chance)
        // critMultiplier 1+ (2.0 = double damage)
        // =========================================
        public static CritResult Calculate(
            int   baseDamage,
            float critChance      = 0.1f,
            float critMultiplier  = 2f)
        {
            bool isCrit = Random.value < critChance;

            return new CritResult
            {
                FinalDamage = isCrit
                    ? Mathf.RoundToInt(baseDamage * critMultiplier)
                    : baseDamage,
                IsCritical  = isCrit
            };
        }

        // =========================================
        // Apply — convenience that modifies
        // an existing DamageInfo in place
        // =========================================
        public static DamageInfo Apply(
            DamageInfo damageInfo,
            float      critChance     = 0.1f,
            float      critMultiplier = 2f)
        {
            var result = Calculate(
                damageInfo.Amount, critChance, critMultiplier);

            return new DamageInfo(
                result.FinalDamage,
                damageInfo.Type,
                result.IsCritical,
                damageInfo.HitPoint,
                damageInfo.Source);
        }
    }
}
