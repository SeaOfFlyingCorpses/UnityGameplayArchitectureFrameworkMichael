using Framework.AI.Systems;
using Framework.StateMachine;

namespace Gameplay.AI.Squad
{
    public class SquadAISystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Squad;

        public void Update(StateContext context)
        {
            if (SquadSystem.Instance == null)
                return;

            SquadSystem.Instance.Tick();
        }
    }
}