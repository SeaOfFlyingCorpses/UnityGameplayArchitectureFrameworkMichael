using Framework.AI.Systems;
using Framework.StateMachine;
using UnityEngine;

namespace Gameplay.AI.LOD
{
    public class AILODSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Utility;

        private const float LOW_DISTANCE = 35f;
        private const float MEDIUM_DISTANCE = 20f;
        private const float HIGH_DISTANCE = 10f;

        public void Update(StateContext context)
        {
            if (context == null || context.Self == null)
                return;

            float distanceScore = GetDistanceScore(context);
            float alertScore = GetAlertScore(context);
            float combatScore = GetCombatScore(context);
            float squadScore = GetSquadScore(context);

            float finalScore =
                distanceScore +
                alertScore +
                combatScore +
                squadScore;

            context.Execution.LOD = EvaluateLOD(finalScore);
        }

        private float GetDistanceScore(StateContext context)
        {
            if (context.Target == null)
                return 0f;

            float dist = Vector3.Distance(
                context.Self.position,
                context.Target.position
            );

            if (dist > LOW_DISTANCE) return 0f;
            if (dist > MEDIUM_DISTANCE) return 1f;
            if (dist > HIGH_DISTANCE) return 2f;

            return 3f;
        }

        private float GetAlertScore(StateContext context)
        {
            return context.AlertLevel switch
            {
                Framework.AI.Alert.AlertLevel.Calm => 0f,
                Framework.AI.Alert.AlertLevel.Suspicious => 1f,
                Framework.AI.Alert.AlertLevel.Alert => 2f,
                Framework.AI.Alert.AlertLevel.Combat => 3f,
                _ => 0f
            };
        }

        private float GetCombatScore(StateContext context)
        {
            if (context.WasHit) return 2f;
            if (context.Suppression > 0.5f) return 1f;
            return 0f;
        }

        private float GetSquadScore(StateContext context)
        {
            if (context.SquadContext?.Leader == null)
                return 0f;

            if (context.SquadContext.Leader.Context == context)
                return 2f;

            return 0.5f;
        }

        private AILODLevel EvaluateLOD(float score)
        {
            if (score >= 6f) return AILODLevel.Critical;
            if (score >= 4f) return AILODLevel.High;
            if (score >= 2f) return AILODLevel.Medium;
            return AILODLevel.Low;
        }

        public bool ShouldRun(StateContext context)
        {
            return true;
        }
    }
}