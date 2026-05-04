using UnityEngine;
using Framework.StateMachine;
using Framework.Core;

namespace Gameplay.AI.Squad
{
    public class SquadSystem : MonoBehaviour
    {
        // =========================================
        // No more "public static Instance"
        // Retrieve via: ServiceLocator.Get<SquadSystem>()
        // =========================================

        public SquadContext GlobalSquad = new SquadContext();

        private void Awake()
        {
            ServiceLocator.Register<SquadSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SquadSystem>();
        }

        // =========================================
        // SELF-TICKING (Step 5a)
        // SquadSystem now ticks itself once per frame
        // via Unity's Update — exactly once, globally.
        // SquadAISystem no longer calls Tick().
        // =========================================
        private void Update()
        {
            if (GlobalSquad.Leader == null)
                return;

            GlobalSquad.UpdateStrategy();
            GlobalSquad.UpdateTarget();
            GlobalSquad.UpdateMoralInfluence();
        }

        // =========================================
        // AGENT REGISTRATION — unchanged
        // =========================================
        public void Register(StateContext context)
        {
            if (context == null)
                return;

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