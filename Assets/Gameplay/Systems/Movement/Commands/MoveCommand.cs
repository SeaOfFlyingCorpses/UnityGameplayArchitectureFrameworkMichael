using Framework.Commands;
using Framework.Movement;
using UnityEngine;

namespace Gameplay.Systems.Movement.Commands
{
    // =========================================
    // MoveCommand
    // Moves an agent using its IMovementStrategy
    // if one is assigned, otherwise falls back
    // to direct transform movement.
    //
    // All states and AI systems use this command
    // — swap strategy on the agent to change
    // how all movement works with zero state changes.
    // =========================================
    public class MoveCommand : ICommand
    {
        private readonly Transform         _transform;
        private readonly Vector3           _direction;
        private readonly float             _speed;
        private readonly IMovementStrategy _strategy;

        private const float WorldBounds = 48f;

        // =========================================
        // Constructor without strategy —
        // uses direct transform (default behaviour)
        // =========================================
        public MoveCommand(Transform transform, Vector3 direction, float speed)
            : this(transform, direction, speed, null) { }

        // =========================================
        // Constructor with strategy —
        // delegates to whatever strategy is set
        // =========================================
        public MoveCommand(
            Transform         transform,
            Vector3           direction,
            float             speed,
            IMovementStrategy strategy)
        {
            _transform = transform;
            _direction = direction;
            _speed     = speed;
            _strategy  = strategy;
        }

        public void Execute()
        {
            if (_transform == null)
                return;

            if (_strategy != null)
            {
                _strategy.MoveInDirection(_transform, _direction, _speed);
                return;
            }

            // Default — direct transform movement
            Vector3 next = _transform.position +
                           _direction.normalized * _speed * Time.deltaTime;

            next.x = Mathf.Clamp(next.x, -WorldBounds, WorldBounds);
            next.z = Mathf.Clamp(next.z, -WorldBounds, WorldBounds);

            _transform.position = next;
        }
    }
}