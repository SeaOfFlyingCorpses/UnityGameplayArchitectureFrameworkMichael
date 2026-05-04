using Framework.StateMachine;
using UnityEngine;

namespace Gameplay.AI.Squad
{
    public class SquadRoleAssigner : MonoBehaviour
    {
        public SquadRoleType roleType;

        public void Assign(StateContext context)
        {
            if (SquadSystem.Instance == null)
                return;

            // find matching member
            var squad = SquadSystem.Instance.GlobalSquad;

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