using UnityEngine;
using Framework.Movement;

namespace Gameplay.Systems.Movement
{
    public class TransformMovementStrategy : IMovementStrategy
    {
        private const float WorldBounds    = 48f;
        private const float GroundDistance = 0.15f;

        public void MoveTo(Transform self, Vector3 destination, float speed)
        {
            Vector3 dir = (destination - self.position).normalized;
            MoveInDirection(self, dir, speed);
        }

        public void MoveInDirection(Transform self, Vector3 direction, float speed)
        {
            if (self == null) return;

            Vector3 next = self.position + direction * speed * Time.deltaTime;
            next.x = Mathf.Clamp(next.x, -WorldBounds, WorldBounds);
            next.z = Mathf.Clamp(next.z, -WorldBounds, WorldBounds);

            self.position = next;
        }

        public void Stop(Transform self) { }

        public bool HasArrived(Transform self, Vector3 destination, float threshold = 0.5f)
            => Vector3.Distance(self.position, destination) <= threshold;

        // =========================================
        // TICK — 3D ground check
        // Writes IsGrounded to context each frame
        // =========================================
        public void Tick(Framework.StateMachine.StateContext context)
        {
            if (context?.Self == null) return;

            // Spherecast downward to check ground
            bool grounded = Physics.SphereCast(
                context.Self.position + Vector3.up * 0.2f,
                0.3f,
                Vector3.down,
                out _,
                GroundDistance);

            context.IsGrounded = grounded;
        }
    }
}