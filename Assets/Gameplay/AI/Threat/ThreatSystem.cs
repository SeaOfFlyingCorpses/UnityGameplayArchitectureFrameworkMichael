using System.Collections.Generic;
using Gameplay.Systems.Health;
using Gameplay.AI.Faction;
using UnityEngine;
using Framework.AI.Systems;
using Framework.StateMachine;
using Gameplay.AI;

namespace Gameplay.AI.Threat
{
    public class ThreatSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Utility;

        // =========================================
        // GET BEST TARGET — filters by team
        // Only returns targets hostile to caller
        // =========================================
        public static Transform GetBestTarget(
            List<Transform> targets,
            Transform self,
            Team selfTeam = Team.Enemy)
        {
            if (targets == null || targets.Count == 0 || self == null)
                return null;

            Transform best      = null;
            float     bestScore = float.MinValue;

            foreach (var t in targets)
            {
                if (t == null) continue;

                // skip friendly targets
                if (AIController.Registry.TryGetValue(t, out var otherCtx))
                {
                    if (otherCtx != null && !TeamRelationship.IsHostile(selfTeam, otherCtx.Team))
                        continue;
                }

                float score = Evaluate(t, self);

                if (score > bestScore)
                {
                    bestScore = score;
                    best      = t;
                }
            }

            return best;
        }

        private static float Evaluate(Transform target, Transform self)
        {
            float distance = Vector3.Distance(self.position, target.position);
            float score    = 10f / (distance + 0.1f);

            var health = target.GetComponent<HealthComponent>();
            if (health != null)
                score += (100 - health.GetHealth().Value) * 0.1f;

            return score;
        }

        // =========================================
        // IAISystem UPDATE
        // =========================================
        public void Update(StateContext context)
        {
            if (context?.PerceptionContext?.VisibleTargets == null)
                return;

            if (context.PerceptionContext.VisibleTargets.Count == 0)
                return;

            var best = GetBestTarget(
                context.PerceptionContext.VisibleTargets,
                context.Self,
                context.Team
            );

            if (best != null)
                context.Target = best;
        }
    }
}