using Framework.AI.Systems;
using Framework.StateMachine;
using Framework.Core;
using Gameplay.AI.Squad;
using UnityEngine;

namespace Gameplay.AI.Moral
{
    public class MoralSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Emotion;

        public void Update(StateContext context)
        {
            if (context == null || context.Perception == null)
                return;

            // Fear from proximity — slowed down to be less aggressive
            if (context.Perception.DistanceToTarget < 5f)
                context.Fear += Time.deltaTime * 0.05f;   // was 0.2f
            else
                context.Fear -= Time.deltaTime * 0.1f;

            // Fear from director intensity
            context.Fear += context.DirectorIntensity * 0.02f * Time.deltaTime; // was 0.1f

            context.Fear   = Mathf.Clamp01(context.Fear);
            context.Morale = 1f - context.Fear;

            // Leader morale bonus
            var squad = ServiceLocator.Get<SquadSystem>()?.GlobalSquad;
            if (squad != null && squad.Leader != null && squad.Leader.Context == context)
                context.Morale = Mathf.Clamp01(context.Morale + 0.1f);
        }
    }
}