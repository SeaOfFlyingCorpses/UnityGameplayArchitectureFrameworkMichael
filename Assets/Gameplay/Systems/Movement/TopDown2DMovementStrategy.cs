using UnityEngine;
using Framework.Movement;

namespace Gameplay.Systems.Movement
{
    // =========================================
    // TopDown2DMovementStrategy
    // 2D top-down movement using Rigidbody2D.
    // Moves in X and Y — no gravity.
    // Good for RPGs, dungeon crawlers,
    // twin-stick shooters, strategy games.
    //
    // Setup:
    //   Add Rigidbody2D to agent
    //   Set Gravity Scale to 0
    //   Set Collision Detection to Continuous
    //   Freeze Z rotation
    //
    //   _context.Movement =
    //       new TopDown2DMovementStrategy(rb2d);
    // =========================================
    public class TopDown2DMovementStrategy : IMovementStrategy
    {
        private readonly Rigidbody2D _rb;
        private readonly bool        _rotateToFace;

        public TopDown2DMovementStrategy(
            Rigidbody2D rb,
            bool        rotateToFace = false)
        {
            _rb           = rb;
            _rotateToFace = rotateToFace;
        }

        public void MoveTo(Transform self, Vector3 destination, float speed)
        {
            if (_rb == null) return;

            Vector2 dir = ((Vector2)destination - _rb.position).normalized;
            MoveInDirection(self, dir, speed);
        }

        public void MoveInDirection(Transform self, Vector3 direction, float speed)
        {
            if (_rb == null) return;

            Vector2 dir2D = new Vector2(direction.x, direction.y).normalized;
            _rb.linearVelocity = dir2D * speed;

            // Optional — rotate sprite to face movement direction
            if (_rotateToFace && dir2D.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(dir2D.y, dir2D.x) * Mathf.Rad2Deg;
                _rb.rotation = angle - 90f;
            }
        }

        public void Stop(Transform self)
        {
            if (_rb == null) return;
            _rb.linearVelocity = Vector2.zero;
        }

        public bool HasArrived(Transform self, Vector3 destination, float threshold = 0.5f)
        {
            return Vector2.Distance(_rb.position, destination) <= threshold;
        }
    }
}
