using Framework.Commands;
using UnityEngine;

namespace Gameplay.Systems.Movement.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Transform _transform;
        private readonly Vector3   _direction;
        private readonly float     _speed;

        // =========================================
        // BOUNDS — keeps agents on the play area
        // Set to match your plane size.
        // Default covers a standard 10x10 Unity plane
        // (which is 10 units = scaled to 100 = 50 half)
        // Adjust WorldBounds to match your scene.
        // =========================================
        private const float WorldBounds = 48f;

        public MoveCommand(Transform transform, Vector3 direction, float speed)
        {
            _transform = transform;
            _direction = direction;
            _speed     = speed;
        }

        public void Execute()
        {
            if (_transform == null)
                return;

            Vector3 next = _transform.position + _direction * _speed * Time.deltaTime;

            // Clamp to world bounds — stops agents walking off the plane
            next.x = Mathf.Clamp(next.x, -WorldBounds, WorldBounds);
            next.z = Mathf.Clamp(next.z, -WorldBounds, WorldBounds);

            _transform.position = next;
        }
    }
}