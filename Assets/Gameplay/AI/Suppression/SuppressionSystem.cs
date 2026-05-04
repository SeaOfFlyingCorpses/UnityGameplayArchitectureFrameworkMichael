using UnityEngine;
using Framework.AI.Systems;
using Framework.StateMachine;

namespace Gameplay.AI.Suppression
{
    public class SuppressionSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Emotion;

        public static void Apply(StateContext context, float amount)
        {
            if (context == null)
                return;

            context.Suppression += amount;
            context.Suppression = Mathf.Clamp01(context.Suppression);
        }

        public void Update(StateContext context)
        {
            if (context == null)
                return;

            context.Suppression -= Time.deltaTime * 0.2f;
            context.Suppression = Mathf.Clamp01(context.Suppression);
        }
    }
}