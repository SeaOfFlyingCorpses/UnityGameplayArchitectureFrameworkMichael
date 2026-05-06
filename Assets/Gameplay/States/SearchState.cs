using System.Collections.Generic;
using Framework.Animation;
using Framework.StateMachine;
using UnityEngine;
using Gameplay.Systems.Movement.Commands;

namespace Gameplay.States
{
    // Moved to Gameplay — uses MoveCommand
    public class SearchState : IState
    {
        private readonly List<Transition> _transitions = new();
        private Vector3 _searchCenter;
        private float   _searchTimer;
        private float   _maxSearchTime = 6f;
        private float   _wanderRadius  = 3f;
        private Vector3 _currentWanderTarget;
        private readonly IState _idleState;

        public SearchState(IState idleState) { _idleState = idleState; }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            _searchCenter = context.Memory != null
                ? context.Memory.LastKnownPosition
                : context.Self.position;
            _searchTimer = 0f;
            PickNewWanderPoint(context);
        }

        public void Update(StateContext context)
        {
            _searchTimer += Time.deltaTime;

            if (_searchTimer > _maxSearchTime ||
                (context.Memory != null && !context.Memory.HasTargetMemory))
            {
                context.Memory?.Forget();
                context.AnimationRequest =
                    new AnimationRequest(AnimationType.Idle);
                return;
            }

            Vector3 toTarget = _currentWanderTarget - context.Self.position;

            if (toTarget.magnitude < 0.3f)
                PickNewWanderPoint(context);
            else
                context.Commands.Enqueue(
                    new MoveCommand(context.Self, toTarget.normalized, 3f, context.Movement));

            context.AnimationRequest = new AnimationRequest(AnimationType.Move);
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;

        private void PickNewWanderPoint(StateContext context)
        {
            Vector2 r = Random.insideUnitCircle * _wanderRadius;
            _currentWanderTarget = _searchCenter + new Vector3(r.x, 0f, r.y);
        }
    }
}