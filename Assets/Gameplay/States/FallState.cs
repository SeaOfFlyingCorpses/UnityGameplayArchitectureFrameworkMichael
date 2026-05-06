using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;

namespace Gameplay.States
{
    // =========================================
    // FallState
    // Active while agent is airborne and falling.
    // Transitions to LandState when grounded.
    // =========================================
    public class FallState : IState
    {
        private readonly List<Transition> _transitions = new();

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            context.AnimationRequest =
                new AnimationRequest(AnimationType.Fall);
        }

        public void Update(StateContext context)
        {
            // Maintain horizontal control during fall
            Vector2 input = context.Input.Move;
            if (Mathf.Abs(input.x) > 0.01f)
            {
                context.Movement?.MoveInDirection(
                    context.Self,
                    new Vector3(input.x, 0f, 0f),
                    4f);
            }

            // IsGrounded updated by IMovementStrategy.Tick() in AIController
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}
