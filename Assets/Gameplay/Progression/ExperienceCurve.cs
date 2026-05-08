using UnityEngine;

namespace Gameplay.Progression
{
    // =========================================
    // ExperienceCurve
    // Pure C# — calculates XP required per level.
    // Three curve types — swap in Inspector.
    //
    // Linear:      flat increase per level
    // Exponential: steeper at high levels (D&D style)
    // Custom:      AnimationCurve from Inspector
    // =========================================
    public enum CurveType
    {
        Linear,
        Exponential,
        Custom
    }

    [System.Serializable]
    public class ExperienceCurve
    {
        public CurveType      type          = CurveType.Exponential;
        public int            baseXP        = 100;  // XP for level 2
        public float          multiplier    = 1.5f; // exponential growth
        public int            linearStep    = 50;   // XP added per level (linear)
        public AnimationCurve customCurve;          // normalized 0-1 input, XP output

        // =========================================
        // XP required to reach this level
        // =========================================
        public int GetRequired(int level)
        {
            if (level <= 1) return 0;

            switch (type)
            {
                case CurveType.Linear:
                    return baseXP + linearStep * (level - 2);

                case CurveType.Exponential:
                    return Mathf.RoundToInt(
                        baseXP * Mathf.Pow(multiplier, level - 2));

                case CurveType.Custom:
                    if (customCurve == null) goto case CurveType.Exponential;
                    float t = (level - 1f) / 99f; // normalize to 0-1
                    return Mathf.RoundToInt(customCurve.Evaluate(t));

                default:
                    return baseXP;
            }
        }

        // =========================================
        // Total XP needed to reach this level from 0
        // =========================================
        public int GetCumulative(int level)
        {
            int total = 0;
            for (int i = 2; i <= level; i++)
                total += GetRequired(i);
            return total;
        }
    }
}
