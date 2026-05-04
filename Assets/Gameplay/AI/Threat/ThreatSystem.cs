using System.Collections.Generic;
using Gameplay.Systems.Health;
using UnityEngine;
using Framework.AI.Systems;
using Framework.StateMachine;

namespace Gameplay.AI.Threat
{
    public class ThreatSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Utility;

        public static Transform GetBestTarget(List<Transform> targets, Transform self)
        {
            if (targets == null || targets.Count == 0 || self == null)
                return null;

            Transform best = null;
            float bestScore = float.MinValue;

            foreach (var t in targets)
            {
                if (t == null) continue;

                float score = Evaluate(t, self);

                if (score > bestScore)
                {
                    bestScore = score;
                    best = t;
                }
            }

            return best;
        }

        private static float Evaluate(Transform target, Transform self)
        {
            float distance = Vector3.Distance(self.position, target.position);

            float score = 0f;
            score += 10f / (distance + 0.1f);

            var health = target.GetComponent<HealthComponent>();
            if (health != null)
            {
                int hp = health.GetHealth().Value;
                score += (100 - hp) * 0.1f;
            }

            return score;
        }

        public void Update(StateContext context)
        {
            if (context?.PerceptionContext?.VisibleTargets == null)
                return;

            if (context.PerceptionContext.VisibleTargets.Count == 0)
                return;

            var best = GetBestTarget(
                context.PerceptionContext.VisibleTargets,
                context.Self
            );

            if (best != null)
                context.Target = best;
        }
    }
}