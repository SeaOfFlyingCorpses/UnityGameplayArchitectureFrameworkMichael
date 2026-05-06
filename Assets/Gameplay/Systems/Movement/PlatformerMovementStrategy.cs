using UnityEngine;
using Framework.Movement;

namespace Gameplay.Systems.Movement
{
    // =========================================
    // PlatformerMovementStrategy
    // 2D platformer movement using Rigidbody2D.
    // Handles horizontal movement and jumping.
    // Gravity handled by Unity physics.
    //
    // Setup:
    //   Add Rigidbody2D to player
    //   Set Gravity Scale to your liking
    //   Set Collision Detection to Continuous
    //   Freeze Z rotation
    //
    //   In AIController or PlayerStateController:
    //   _context.Movement =
    //       new PlatformerMovementStrategy(rb2d);
    // =========================================
    public class PlatformerMovementStrategy : IMovementStrategy
    {
        private readonly Rigidbody2D _rb;
        private readonly float       _jumpForce;
        private readonly LayerMask   _groundLayer;

        private bool _isGrounded;

        public bool IsGrounded => _isGrounded;

        public PlatformerMovementStrategy(
            Rigidbody2D rb,
            float       jumpForce   = 10f,
            LayerMask   groundLayer = default)
        {
            _rb          = rb;
            _jumpForce   = jumpForce;
            _groundLayer = groundLayer;
        }

        // =========================================
        // IMovementStrategy
        // =========================================
        public void MoveTo(Transform self, Vector3 destination, float speed)
        {
            if (_rb == null) return;

            float dir = destination.x > self.position.x ? 1f : -1f;
            MoveInDirection(self, new Vector3(dir, 0f, 0f), speed);
        }

        public void MoveInDirection(Transform self, Vector3 direction, float speed)
        {
            if (_rb == null) return;

            // Only affect horizontal — let physics handle vertical
            _rb.linearVelocity = new Vector2(
                direction.x * speed,
                _rb.linearVelocity.y);

            // Flip sprite based on direction
            if (direction.x != 0f)
            {
                Vector3 scale = self.localScale;
                scale.x       = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
                self.localScale = scale;
            }
        }

        public void Stop(Transform self)
        {
            if (_rb == null) return;
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        }

        public bool HasArrived(Transform self, Vector3 destination, float threshold = 0.5f)
        {
            return Mathf.Abs(self.position.x - destination.x) <= threshold;
        }

        // =========================================
        // PLATFORMER-SPECIFIC
        // =========================================
        public void Jump()
        {
            if (_rb == null || !_isGrounded) return;

            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _isGrounded = false;
        }

        // =========================================
        // IMovementStrategy.Tick
        // Updates grounded state and writes to context
        // Called each frame by AIController
        // =========================================
        public void Tick(Framework.StateMachine.StateContext context)
        {
            if (_rb == null) return;

            var hit = Physics2D.BoxCast(
                context.Self.position,
                new Vector2(0.8f, 0.1f),
                0f,
                Vector2.down,
                0.15f,
                _groundLayer);

            _isGrounded        = hit.collider != null;
            context.IsGrounded = _isGrounded;

            // Wall check
            var wallRight = Physics2D.BoxCast(
                context.Self.position,
                new Vector2(0.1f, 0.8f),
                0f,
                Vector2.right,
                0.1f,
                _groundLayer);

            var wallLeft = Physics2D.BoxCast(
                context.Self.position,
                new Vector2(0.1f, 0.8f),
                0f,
                Vector2.left,
                0.1f,
                _groundLayer);

            context.IsOnWall = wallRight.collider != null ||
                               wallLeft.collider  != null;
        }

        // Keep for backward compat
        public void UpdateGroundCheck(Transform self)
        {
            if (_rb == null) return;

            var hit = Physics2D.BoxCast(
                self.position,
                new Vector2(0.8f, 0.1f),
                0f,
                Vector2.down,
                0.15f,
                _groundLayer);

            _isGrounded = hit.collider != null;
        }

        public void SetVelocityY(float vy)
        {
            if (_rb == null) return;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, vy);
        }
    }
}
