using System.Collections.Generic;
using Framework.Animation;
using Framework.StateMachine;
using UnityEngine;
using Framework.StateMachine.Conditions;
using Gameplay.Systems.Movement.Commands;

namespace Gameplay.States
{
    public class MoveState : IState
    {
        private readonly List<Transition> _transitions = new();

        public void Init(IState idleState, IState attackState)
        {
            _transitions.Add(new Transition(new MoveReleasedCondition(), idleState));
            _transitions.Add(new Transition(new AttackPressedCondition(), attackState));
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            context.AnimationRequest =
                new AnimationRequest(AnimationType.Move);
        }

        public void Update(StateContext context)
        {
            Vector2 input = context.Input.Move;

            if (input.sqrMagnitude < 0.01f)
                return;

            // Camera-relative movement — works for all camera modes
            // including isometric, topdown, 3rd person
            Vector3 moveDir = GetCameraRelativeDirection(input);

            context.Commands.Enqueue(
                new MoveCommand(context.Self, moveDir, 5f, context.Movement));

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Move);
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;

        private Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            var cam = UnityEngine.Camera.main;

            if (cam == null)
                return new Vector3(input.x, 0f, input.y).normalized;

            Vector3 forward = cam.transform.forward;
            Vector3 right   = cam.transform.right;

            forward.y = 0f;
            right.y   = 0f;

            forward.Normalize();
            right.Normalize();

            return (forward * input.y + right * input.x).normalized;
        }
    }
}