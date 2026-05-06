using UnityEngine;
using UnityEngine.AI;
using Framework.Movement;

namespace Gameplay.Systems.Movement
{
    // =========================================
    // NavMeshMovementStrategy
    // Uses Unity NavMeshAgent for pathfinding.
    // Agents navigate around obstacles properly.
    //
    // Setup:
    //   1. Bake NavMesh on your scene geometry
    //      (Window → AI → Navigation → Bake)
    //   2. Add NavMeshAgent component to agent
    //   3. In AIController.BindSystems() add:
    //      var navAgent = GetComponent<NavMeshAgent>();
    //      if (navAgent != null)
    //          _context.Movement =
    //              new NavMeshMovementStrategy(navAgent);
    //
    // The agent's NavMeshAgent speed is overridden
    // by the speed passed from states/commands.
    // =========================================
    public class NavMeshMovementStrategy : IMovementStrategy
    {
        private readonly NavMeshAgent _agent;

        public NavMeshMovementStrategy(NavMeshAgent agent)
        {
            _agent = agent;

            if (_agent != null)
            {
                // Let strategy control movement
                _agent.updateRotation = true;
                _agent.updatePosition = true;
            }
        }

        public void MoveTo(Transform self, Vector3 destination, float speed)
        {
            if (_agent == null || !_agent.isActiveAndEnabled) return;

            _agent.speed = speed;
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }

        public void MoveInDirection(Transform self, Vector3 direction, float speed)
        {
            if (_agent == null || !_agent.isActiveAndEnabled) return;

            // For directional movement — move to a point along the direction
            Vector3 destination = self.position + direction.normalized * speed;
            _agent.speed = speed;
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }

        public void Stop(Transform self)
        {
            if (_agent == null || !_agent.isActiveAndEnabled) return;

            _agent.isStopped = true;
            _agent.velocity  = Vector3.zero;
        }

        public bool HasArrived(Transform self, Vector3 destination, float threshold = 0.5f)
        {
            if (_agent == null) return false;

            return !_agent.pathPending &&
                   _agent.remainingDistance <= threshold;
        }
    }
}
