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

        // Tight formation — used during patrol/search
        private const float TightArrivalThreshold = 0.5f;
        private const float TightMoveSpeed        = 3.5f;

        // Loose formation — used during combat/engage
        // Agents only correct if very far from slot
        private const float LooseArrivalThreshold = 4f;
        private const float LooseMoveSpeed        = 2f;

        public bool ShouldRun(StateContext context)
        {
            // Skip if no squad or no formation
            if (context.SquadContext?.Formation == null)
                return false;

            // Skip during retreat — agents flee freely
            if (context.SquadStrategy == SquadStrategy.Retreat)
                return false;

            return true;
        }

        public void Update(StateContext context)
        {
            var squad     = context.SquadContext as SquadContext;
            var formation = squad?.TypedFormation;

            if (formation?.Leader == null)
                return;

            // Determine formation tightness by strategy
            bool isEngaging =
                context.SquadStrategy == SquadStrategy.Engage;

            float arrivalThreshold = isEngaging
                ? LooseArrivalThreshold
                : TightArrivalThreshold;

            float moveSpeed = isEngaging
                ? LooseMoveSpeed
                : TightMoveSpeed;

            // Find this agent's index
            int index = -1;
            for (int i = 0; i < squad.Members.Count; i++)
            {
                if (squad.Members[i].Context == context)
                {
                    index = i;
                    break;
                }
            }

            // Leader holds position — only followers move
            if (index <= 0)
                return;

            Vector3 offset         = FormationSystem.GetOffset(index, formation);
            Vector3 targetPosition = formation.Leader.position + offset;
            Vector3 toTarget       = targetPosition - context.Self.position;

            // Skip if close enough
            if (toTarget.magnitude <= arrivalThreshold)
                return;

            // During combat — only correct if very far off
            // This lets agents fight freely but drift back gradually
            context.Commands.Enqueue(
                new MoveCommand(
                    context.Self,
                    toTarget.normalized,
                    moveSpeed,
                    context.Movement));
        }
    }
}