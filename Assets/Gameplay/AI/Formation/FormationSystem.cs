using UnityEngine;

namespace Gameplay.AI.Formation
{
    public static class FormationSystem
    {
        public static Vector3 GetOffset(int index, FormationData formation)
        {
            if (formation == null || formation.Leader == null)
                return Vector3.zero;

            switch (formation.Type)
            {
                case FormationType.Line:
                    return formation.Leader.right * formation.Spacing * index;

                case FormationType.Wedge:
                    float side = (index % 2 == 0 ? 1 : -1);
                    float depth = index * formation.Spacing;
                    return (formation.Leader.right * side + formation.Leader.forward).normalized * depth;

                case FormationType.Circle:
                    float angle = index * 30f;
                    return Quaternion.Euler(0, angle, 0) * Vector3.forward * formation.Spacing;

                default:
                    return Vector3.zero;
            }
        }
    }
}