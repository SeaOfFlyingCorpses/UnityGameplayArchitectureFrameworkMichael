using UnityEngine;
using Framework.StateMachine;
using Framework.AI.Faction;
using Framework.Core;

namespace Gameplay.AI.Squad
{
    public class SquadSystem : MonoBehaviour
    {
        public SquadContext EnemySquad  = new SquadContext();
        public SquadContext AllySquad   = new SquadContext();
        public SquadContext PlayerSquad = new SquadContext();

        public SquadContext GlobalSquad => EnemySquad;

        private void Awake()
        {
            ServiceLocator.Register<SquadSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SquadSystem>();
        }

        private void Update()
        {
            TickSquad(EnemySquad);
            TickSquad(AllySquad);
            TickSquad(PlayerSquad);
        }

        private void TickSquad(SquadContext squad)
        {
            if (squad.Members.Count == 0) return;

            // Clean up any destroyed members first
            squad.Members.RemoveAll(m =>
                m?.Context?.Self == null ||
                !m.Context.Self.gameObject.activeInHierarchy == false &&
                m.Context.Commands == null);

            if (squad.Leader == null) return;

            squad.UpdateStrategy();
            squad.UpdateTarget();
            squad.UpdateMoralInfluence();
        }

        public void Register(StateContext context)
        {
            if (context == null) return;

            var squad = GetSquad(context.Team);
            if (squad == null) return;

            foreach (var m in squad.Members)
                if (m.Context == context) return;

            squad.Members.Add(new SquadMemberData { Context = context });
            squad.AssignRoles();
        }

        public void Unregister(StateContext context)
        {
            if (context == null) return;

            UnregisterFrom(context, EnemySquad);
            UnregisterFrom(context, AllySquad);
            UnregisterFrom(context, PlayerSquad);
        }

        private void UnregisterFrom(
            StateContext context, SquadContext squad)
        {
            int removed = squad.Members.RemoveAll(
                m => m.Context == context);

            if (removed == 0) return;

            if (squad.TypedLeader?.Context == context)
                squad.Leader = squad.Members.Count > 0
                    ? squad.Members[0] : null;

            squad.AssignRoles();
        }

        public SquadContext GetSquad(Team team)
        {
            switch (team)
            {
                case Team.Ally:   return AllySquad;
                case Team.Player: return PlayerSquad;
                default:          return EnemySquad;
            }
        }

        public Vector3 GetTargetPosition(Team team)
            => GetSquad(team).GetTargetPosition();
    }
}