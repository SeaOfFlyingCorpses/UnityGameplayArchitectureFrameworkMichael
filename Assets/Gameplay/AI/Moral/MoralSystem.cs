using Framework.AI.Systems;
using Framework.StateMachine;
using UnityEngine;

namespace Gameplay.AI.Moral
{
    public class MoralSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Emotion;

        public void Update(StateContext context)
        {
            if (context == null)
                return;

            if (context.Perception == null)
                return;

            if (context.Perception.DistanceToTarget < 5f)
                context.Fear += Time.deltaTime * 0.2f;
            else
                context.Fear -= Time.deltaTime * 0.1f;

            if (Gameplay.AI.Director.AIDirector.Instance != null)
            {
                float intensity = Gameplay.AI.Director.AIDirector.Instance.State.Intensity;
                context.Fear += intensity * 0.1f * Time.deltaTime;
            }

            context.Fear = Mathf.Clamp01(context.Fear);
            context.Morale = 1f - context.Fear;

            var squad = Gameplay.AI.Squad.SquadSystem.Instance?.GlobalSquad;

            if (squad != null && squad.Leader != null && squad.Leader.Context == context)
            {
                context.Morale = Mathf.Clamp01(context.Morale + 0.1f);
            }
        }
    }
}