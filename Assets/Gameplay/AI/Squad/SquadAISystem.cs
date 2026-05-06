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
            var squadSystem = ServiceLocator.Get<SquadSystem>();
            if (squadSystem == null) return;

            // Get the squad that matches this agent's team
            var squad = squadSystem.GetSquad(context.Team);
            if (squad == null) return;

            if (context.SquadContext != null)
            {
                context.SquadContext.CurrentStrategy = squad.CurrentStrategy;
                context.SquadContext.CurrentTarget   = squad.CurrentTarget;
            }

            context.SquadStrategy = squad.CurrentStrategy;
        }
    }
}