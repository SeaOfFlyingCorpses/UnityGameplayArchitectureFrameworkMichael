using UnityEngine;

namespace Gameplay.Combat
{
    public static class TargetingSystem
    {
        public static CombatTarget? GetTarget(Transform self, float radius, LayerMask mask)
        {
            Collider[] hits = Physics.OverlapSphere(self.position, radius, mask);

            float bestDistance = float.MaxValue;
            CombatTarget? best = null;

            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent(out Targetable targetable))
                    continue;

                if (targetable.Health == null)
                    continue;

                float dist = Vector3.Distance(self.position, hit.transform.position);

                if (dist < bestDistance)
                {
                    bestDistance = dist;

                    best = new CombatTarget
                    {
                        Transform = hit.transform,
                        Health = targetable.Health
                    };
                }
            }

            return best;
        }
    }
}