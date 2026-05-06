using UnityEngine;

namespace Framework.Movement
{
    // =========================================
    // IMovementStrategy
    // Defines how an agent physically moves.
    // Swap implementations per agent without
    // touching any state or command code.
    //
    // Built-in implementations:
    //   TransformMovementStrategy — direct transform
    //   NavMeshMovementStrategy   — Unity NavMesh
    //
    // Usage — assign on StateContext:
    //   context.Movement = new NavMeshMovementStrategy(agent);
    //   context.Movement = new TransformMovementStrategy();
    // =========================================
    public interface IMovementStrategy
    {
        // Move toward a world position at given speed
        void MoveTo(Transform self, Vector3 destination, float speed);

        // Move in a direction (for strafing, fleeing etc)
        void MoveInDirection(Transform self, Vector3 direction, float speed);

        // Stop all movement
        void Stop(Transform self);

        // True when agent has reached its destination
        bool HasArrived(Transform self, Vector3 destination, float threshold = 0.5f);

        // =========================================
        // TICK — called each frame by AIController
        // Write physics state to context
        // (IsGrounded, IsOnWall etc.)
        // Optional — default implementation does nothing
        // =========================================
        void Tick(Framework.StateMachine.StateContext context) { }
    }
}