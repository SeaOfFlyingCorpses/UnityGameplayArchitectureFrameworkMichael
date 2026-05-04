using System.Collections.Generic;
using UnityEngine;
using Gameplay.Systems.Movement.Commands;

namespace Framework.StateMachine.States
{
    public class SearchState : IState
    {
        private readonly List<Transition> _transitions = new();

        private Vector3 _searchCenter;
        private float _searchTimer;
        private float _maxSearchTime = 6f;

        private float _wanderRadius = 3f;
        private float _nextWanderTime;

        private Vector3 _currentWanderTarget;

        private IState _idleState;

        public SearchState(IState idleState)
        {
            _idleState = idleState;
        }

        public void Enter(StateContext context)
        {
            _searchCenter = context.Memory.LastKnownPosition;
            _searchTimer = 0f;

            PickNewWanderPoint(context);
        }

        public void Update(StateContext context)
        {
            _searchTimer += Time.deltaTime;

            if (_searchTimer > _maxSearchTime || !context.Memory.HasTargetMemory)
            {
                context.Memory.Forget();

                context.AnimationRequest =
                    new Framework.Animation.AnimationRequest(
                        Framework.Animation.AnimationType.Idle
                    );

                return;
            }

            Vector3 toTarget = _currentWanderTarget - context.Self.position;

            if (toTarget.magnitude < 0.3f)
            {
                PickNewWanderPoint(context);
            }
            else
            {
                context.Commands.Enqueue(
                    new Gameplay.Systems.Movement.Commands.MoveCommand(
                        context.Self,
                        toTarget.normalized,
                        3f
                    )
                );
            }

            context.AnimationRequest =
                new Framework.Animation.AnimationRequest(
                    Framework.Animation.AnimationType.Move
                );
        }

        public void Exit()
        {
        }

        public List<Transition> GetTransitions() => _transitions;

      
        // SEARCH LOGIC
        private void PickNewWanderPoint(StateContext context)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _wanderRadius;

            _currentWanderTarget = _searchCenter + new Vector3(
                randomCircle.x,
                0,
                randomCircle.y
            );
        }
    }
}