using UnityEngine;
using Framework.AI.Systems;
using Framework.StateMachine;

namespace Gameplay.AI.Systems
{
    // =========================================
    // LedgeDetectionSystem
    // Detects when a 2D agent is about to walk
    // off a ledge and stops them.
    //
    // Writes context.IsOnWall = true when a
    // ledge is detected in the movement direction.
    // PatrolState and ChaseState read IsOnWall
    // to stop horizontal movement.
    //
    // Only runs when agent is grounded and moving.
    // Uses a downward BoxCast slightly ahead of
    // the agent to detect floor presence.
    //
    // Setup:
    //   Register in AISystemsBootstrap for all
    //   2D platformer agents.
    //   Set groundLayer to match your floor layer.
    // =========================================
    public class LedgeDetectionSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Utility;

        private readonly LayerMask _groundLayer;
        private readonly float     _checkDistance;
        private readonly float     _checkAhead;

        // =========================================
        // checkAhead — how far ahead to check
        //              (half agent width + margin)
        // checkDistance — how far down to cast
        //                 (step height + margin)
        // =========================================
        public LedgeDetectionSystem(
            LayerMask groundLayer,
            float     checkAhead    = 0.6f,
            float     checkDistance = 0.5f)
        {
            _groundLayer   = groundLayer;
            _checkDistance = checkDistance;
            _checkAhead    = checkAhead;
        }

        public bool ShouldRun(StateContext context)
        {
            // Only relevant for grounded 2D agents
            return context.IsGrounded;
        }

        public void Update(StateContext context)
        {
            if (context?.Self == null) return;

            // Determine movement direction from velocity
            // Default to right if not moving
            var rb2d = context.Self.GetComponent<Rigidbody2D>();
            if (rb2d == null) return;

            float moveDir = rb2d.linearVelocity.x;

            // Not moving horizontally — no ledge risk
            if (Mathf.Abs(moveDir) < 0.1f) return;

            float sign       = Mathf.Sign(moveDir);
            Vector2 origin   = new Vector2(
                context.Self.position.x + sign * _checkAhead,
                context.Self.position.y);

            // Cast downward to detect floor ahead
            var hit = Physics2D.BoxCast(
                origin,
                new Vector2(0.1f, 0.1f),
                0f,
                Vector2.down,
                _checkDistance,
                _groundLayer);

            // No floor ahead — ledge detected
            // Set IsOnWall so states can react
            bool ledgeAhead = hit.collider == null;
            context.IsOnWall = ledgeAhead;
        }
    }
}
