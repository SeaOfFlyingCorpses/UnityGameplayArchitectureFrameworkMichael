using Framework.AI.Systems;
using Framework.StateMachine;
using Framework.AI.Squad;
using Gameplay.AI.Formation;
using Gameplay.Systems.Movement.Commands;
using UnityEngine;

namespace Gameplay.AI.Squad
{
    public class FormationAISystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Squad;

        private const float MoveSpeed        = 3.5f;
        private const float ArrivalThreshold = 0.5f;

        public bool ShouldRun(StateContext context)
        {
            var squad = context.SquadContext as SquadContext;
            if (squad?.TypedFormation == null) return false;

            if (context.SquadStrategy == SquadStrategy.Engage ||
                context.SquadStrategy == SquadStrategy.Retreat)
                return false;

            return true;
        }

        public void Update(StateContext context)
        {
            var squad     = context.SquadContext as SquadContext;
            var formation = squad?.TypedFormation;

            if (formation?.Leader == null) return;

            int index = -1;
            for (int i = 0; i < squad.Members.Count; i++)
            {
                if (squad.Members[i].Context == context)
                {
                    index = i;
                    break;
                }
            }

            if (index <= 0) return;

            Vector3 offset         = FormationSystem.GetOffset(index, formation);
            Vector3 targetPosition = formation.Leader.position + offset;
            Vector3 toTarget       = targetPosition - context.Self.position;

            if (toTarget.magnitude <= ArrivalThreshold) return;

            context.Commands.Enqueue(
                new MoveCommand(context.Self, toTarget.normalized, MoveSpeed));
        }
    }
}