using Framework.AI.Systems;
using Framework.StateMachine;
using Framework.AI.Squad;
using Framework.Core;

namespace Gameplay.AI.Squad
{
    public class SquadAISystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Squad;

        public void Update(StateContext context)
        {
            var globalSquad = ServiceLocator.Get<SquadSystem>()?.GlobalSquad;

            if (globalSquad == null) return;

            if (context.SquadContext != null)
            {
                context.SquadContext.CurrentStrategy = globalSquad.CurrentStrategy;
                context.SquadContext.CurrentTarget   = globalSquad.CurrentTarget;
            }

            context.SquadStrategy = globalSquad.CurrentStrategy;
        }
    }
}