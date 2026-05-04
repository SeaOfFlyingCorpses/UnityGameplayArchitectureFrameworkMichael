using Framework.AI.Systems;
using Framework.StateMachine;
using Framework.Core;

namespace Gameplay.AI.Squad
{
    // =========================================
    // SquadAISystem
    // Syncs global squad state into each agent's
    // StateContext every frame.
    //
    // SquadSystem.Update() ticks the squad once
    // globally via MonoBehaviour — this system
    // only pushes the result down into context.
    // =========================================
    public class SquadAISystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Squad;

        public void Update(StateContext context)
        {
            var squad = ServiceLocator.Get<SquadSystem>()?.GlobalSquad;

            if (squad == null)
                return;

            // Sync into SquadContext sub-context if present
            if (context.SquadContext != null)
            {
                context.SquadContext.CurrentStrategy = squad.CurrentStrategy;
                context.SquadContext.CurrentTarget   = squad.CurrentTarget;
            }

            // Also write flat field so framework states
            // can read strategy without importing SquadContext
            context.SquadStrategy = squad.CurrentStrategy;
        }
    }
}