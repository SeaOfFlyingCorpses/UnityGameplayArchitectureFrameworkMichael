using Framework.StateMachine;
using Framework.Core;
using UnityEngine;

namespace Gameplay.AI.Squad
{
    public class SquadRoleAssigner : MonoBehaviour
    {
        public SquadRoleType roleType;

        public void Assign(StateContext context)
        {
            var squad = ServiceLocator.Get<SquadSystem>()?.GlobalSquad;

            if (squad == null)
                return;

            foreach (var m in squad.Members)
            {
                if (m.Context == context)
                {
                    m.Role = new SquadRole(roleType);
                    return;
                }
            }
        }
    }
}