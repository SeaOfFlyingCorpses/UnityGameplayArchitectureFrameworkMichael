using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;
using Gameplay.Systems.Movement.Commands;

namespace Gameplay.States
{
    // =========================================
    // PatrolState
    // Agent moves between waypoints or wanders
    // randomly when no waypoints are set.
    //
    // Transitions out via factory conditions:
    //   → ChaseState when target spotted
    //   → IdleState  when patrol finishes
    //
    // Usage — with waypoints:
    //   var patrol = new PatrolState(waypoints);
    //
    // Usage — random wander:
    //   var patrol = new PatrolState();
    // =========================================
    public class PatrolState : IState
    {
        private readonly List<Transition> _transitions = new();

        private readonly Transform[] _waypoints;
        private readonly float       _speed;
        private readonly float       _waitTime;
        private readonly float       _wanderRadius;

        private int     _currentWaypoint;
        private float   _waitTimer;
        private bool    _waiting;
        private Vector3 _wanderTarget;
        private bool    _hasWanderTarget;

        // =========================================
        // Waypoint patrol
        // =========================================
        public PatrolState(
            Transform[] waypoints,
            float       speed       = 2.5f,
            float       waitTime    = 1.5f)
        {
            _waypoints    = waypoints;
            _speed        = speed;
            _waitTime     = waitTime;
            _wanderRadius = 0f;
        }

        // =========================================
        // Random wander — no waypoints needed
        // =========================================
        public PatrolState(
            float speed        = 2.5f,
            float waitTime     = 1.5f,
            float wanderRadius = 8f)
        {
            _waypoints    = null;
            _speed        = speed;
            _waitTime     = waitTime;
            _wanderRadius = wanderRadius;
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            _waiting          = false;
            _waitTimer        = 0f;
            _hasWanderTarget  = false;

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Move);
        }

        public void Update(StateContext context)
        {
            if (_waypoints != null && _waypoints.Length > 0)
                UpdateWaypointPatrol(context);
            else
                UpdateWander(context);
        }

        private void UpdateWaypointPatrol(StateContext context)
        {
            if (_waiting)
            {
                _waitTimer += Time.deltaTime;
                context.AnimationRequest =
                    new AnimationRequest(AnimationType.Idle);

                if (_waitTimer >= _waitTime)
                {
                    _waiting   = false;
                    _waitTimer = 0f;
                    _currentWaypoint =
                        (_currentWaypoint + 1) % _waypoints.Length;
                }
                return;
            }

            var target = _waypoints[_currentWaypoint];
            if (target == null) return;

            Vector3 dir      = target.position - context.Self.position;
            float   distance = dir.magnitude;

            if (distance < 0.5f)
            {
                _waiting = true;
                return;
            }

            context.Commands.Enqueue(
                new MoveCommand(context.Self, dir.normalized, _speed, context.Movement));

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Move);
        }

        private const float WorldBounds = 48f;

        private void UpdateWander(StateContext context)
        {
            if (_waiting)
            {
                _waitTimer += Time.deltaTime;
                context.AnimationRequest =
                    new AnimationRequest(AnimationType.Idle);

                if (_waitTimer >= _waitTime)
                {
                    _waiting          = false;
                    _waitTimer        = 0f;
                    _hasWanderTarget  = false;
                }
                return;
            }

            if (!_hasWanderTarget)
            {
                Vector2 rand   = Random.insideUnitCircle * _wanderRadius;
                Vector3 target = context.Self.position +
                                 new Vector3(rand.x, 0f, rand.y);

                // Clamp wander target to world bounds
                target.x = Mathf.Clamp(target.x, -WorldBounds, WorldBounds);
                target.z = Mathf.Clamp(target.z, -WorldBounds, WorldBounds);

                _wanderTarget    = target;
                _hasWanderTarget = true;
            }

            Vector3 toTarget = _wanderTarget - context.Self.position;
            float   dist     = toTarget.magnitude;

            if (dist < 0.5f)
            {
                _waiting         = true;
                _hasWanderTarget = false;
                return;
            }

            // Stop at ledges — LedgeDetectionSystem writes IsOnWall
            if (context.IsOnWall)
            {
                _hasWanderTarget = false;
                _waiting         = true;
                return;
            }

            context.Commands.Enqueue(
                new MoveCommand(context.Self, toTarget.normalized,
                    _speed, context.Movement));

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Move);
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}