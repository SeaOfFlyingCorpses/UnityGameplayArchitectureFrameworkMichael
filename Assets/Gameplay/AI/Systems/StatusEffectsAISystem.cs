using Framework.AI.Systems;
using Framework.StateMachine;
using UnityEngine;

namespace Gameplay.AI.Systems
{
    // =========================================
    // StatusEffectsAISystem
    // Ticks all active status effects each frame
    // via AISystemManager.
    //
    // Register in AISystemsBootstrap:
    //   manager.Register(new StatusEffectsAISystem());
    //
    // Zero coupling — reads/writes only through
    // IStatusEffectSystem on StateContext.
    // =========================================
    public class StatusEffectsAISystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Utility;

        public bool ShouldRun(StateContext context)
            => context.StatusEffects != null &&
               context.StatusEffects.Active.Count > 0;

        public void Update(StateContext context)
        {
            context.StatusEffects?.Tick(context, Time.deltaTime);
        }
    }
}
