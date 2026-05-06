using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;
using Framework.StateMachine.Conditions;

namespace Gameplay.States
{
    // =========================================
    // JumpState
    // Triggers a jump via PlatformerMovementStrategy.
    // Transitions to FallState when rising stops.
    // =========================================
    public class JumpState : IState
    {
        private readonly List<Transition> _transitions = new();

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            context.AnimationRequest =
                new AnimationRequest(AnimationType.Jump);

            // Trigger jump via strategy
            var platformer = context.Movement
                as Gameplay.Systems.Movement.PlatformerMovementStrategy;
            platformer?.Jump();
        }

        public void Update(StateContext context)
        {
            // Apply horizontal movement during jump
            Vector2 input = context.Input.Move;
            if (Mathf.Abs(input.x) > 0.01f)
            {
                context.Movement?.MoveInDirection(
                    context.Self,
                    new Vector3(input.x, 0f, 0f),
                    5f);
            }
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}
