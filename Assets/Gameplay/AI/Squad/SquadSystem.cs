using UnityEngine;
using Framework.StateMachine;

namespace Gameplay.AI.Squad
{
    public class SquadSystem : MonoBehaviour
    {
        public static SquadSystem Instance;

        public SquadContext GlobalSquad = new SquadContext();

        private void Awake()
        {
            Instance = this;
        }

        public void Tick()
        {
            if (GlobalSquad.Leader == null)
                return;

            GlobalSquad.UpdateStrategy();
            GlobalSquad.UpdateTarget();
            GlobalSquad.UpdateMoralInfluence();
        }

        public void Register(StateContext context)
        {
            if (context == null)
                return;

            // ✅ WRAP StateContext INTO SquadMemberData
            foreach (var m in GlobalSquad.Members)
                if (m.Context == context)
                    return;

            GlobalSquad.Members.Add(new SquadMemberData
            {
                Context = context
            });

            GlobalSquad.AssignRoles();
        }

        public void Unregister(StateContext context)
        {
            if (context == null)
                return;

            GlobalSquad.Members.RemoveAll(m => m.Context == context);

            if (GlobalSquad.Leader != null &&
                GlobalSquad.Leader.Context == context)
            {
                GlobalSquad.Leader =
                    GlobalSquad.Members.Count > 0
                        ? GlobalSquad.Members[0]
                        : null;
            }

            GlobalSquad.AssignRoles();
        }

        public Vector3 GetTargetPosition()
        {
            return GlobalSquad.GetTargetPosition();
        }
    }
}