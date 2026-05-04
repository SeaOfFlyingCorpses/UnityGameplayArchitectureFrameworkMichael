using System.Collections.Generic;
using UnityEngine;
using Framework.AI.Alert;
using Gameplay.Systems.Movement.Commands;

namespace Framework.StateMachine.States
{
    public class ChaseState : IState
    {
        private readonly List<Transition> _transitions = new();

        private readonly IState _attackState;
        private readonly IState _searchState;

        public ChaseState(IState attackState, IState searchState)
        {
            _attackState = attackState;
            _searchState = searchState;
        }

        public void Enter(StateContext context)
        {
            context.AnimationRequest =
                new Framework.Animation.AnimationRequest(
                    Framework.Animation.AnimationType.Move
                );
        }

        public void Update(StateContext context)
        {
            if (context.Target == null)
                return;

            Vector3 direction = context.Target.position - context.Self.position;
            float distance = direction.magnitude;

            if (context.AlertLevel == Framework.AI.Alert.AlertLevel.Calm)
                return;

            float speed = 4f;

            if (context.AlertLevel == Framework.AI.Alert.AlertLevel.Suspicious)
                speed = 2.5f;

            if (context.AlertLevel == Framework.AI.Alert.AlertLevel.Combat)
                speed = 5.5f;

            if (distance <= 2f)
            {
                context.AnimationRequest =
                    new Framework.Animation.AnimationRequest(
                        Framework.Animation.AnimationType.Attack
                    );

                return;
            }

            context.Commands.Enqueue(
                new Gameplay.Systems.Movement.Commands.MoveCommand(
                    context.Self,
                    direction.normalized,
                    speed
                )
            );

            // MEMORY WRITE (SAFE)
            if (context.Memory != null && context.Target != null)
                context.Memory.Remember(context.Target.position, Time.time);

            context.AnimationRequest =
                new Framework.Animation.AnimationRequest(
                    Framework.Animation.AnimationType.Move
                );
        }

        public void Exit() { }

        public List<Transition> GetTransitions() => _transitions;
    }
}