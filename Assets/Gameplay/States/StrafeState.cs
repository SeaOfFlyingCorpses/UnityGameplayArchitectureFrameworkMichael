using System.Collections.Generic;
using Framework.Animation;
using Framework.StateMachine;
using UnityEngine;
using Gameplay.Systems.Movement.Commands;

namespace Gameplay.States
{
    // Moved to Gameplay — uses MoveCommand
    public class StrafeState : IState
    {
        private readonly List<Transition> _transitions = new();
        private float _strafeDirection = 1f;
        private float _changeDirTimer;
        private float _strafeSpeed     = 3f;
        private float _desiredDistance = 2.5f;

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context) => _changeDirTimer = 0f;

        public void Update(StateContext context)
        {
            if (context.Target == null) return;

            Vector3 toTarget  = context.Target.position - context.Self.position;
            float   distance  = toTarget.magnitude;
            Vector3 forward   = toTarget.normalized;
            Vector3 strafeDir = Vector3.Cross(Vector3.up, forward) * _strafeDirection;

            _changeDirTimer += Time.deltaTime;
            if (_changeDirTimer > 2f) { _strafeDirection *= -1f; _changeDirTimer = 0f; }

            Vector3 moveDir = strafeDir;
            if (distance > _desiredDistance + 0.5f) moveDir += forward;
            if (distance < _desiredDistance - 0.5f) moveDir -= forward;

            context.Commands.Enqueue(
                new MoveCommand(context.Self, moveDir.normalized, _strafeSpeed));

            context.AnimationRequest = new AnimationRequest(AnimationType.Move);
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}
