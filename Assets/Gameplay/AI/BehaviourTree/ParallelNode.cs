using System.Collections.Generic;
using Framework.AI.BehaviourTree;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // ParallelNode
    // Ticks ALL children every frame.
    // Policy controls when it succeeds/fails.
    //
    // RequireAll  — succeed when all succeed
    // RequireOne  — succeed when any succeeds
    //
    // Use for: "Do A and B simultaneously"
    // Example: Move toward target AND play
    //          alert animation at same time
    // =========================================
    public enum ParallelPolicy
    {
        RequireAll,  // succeed only when all children succeed
        RequireOne   // succeed when any child succeeds
    }

    public class ParallelNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly List<IBehaviourNode> _children  = new();
        private readonly ParallelPolicy       _policy;

        public ParallelNode(
            string          name   = "Parallel",
            ParallelPolicy  policy = ParallelPolicy.RequireAll)
        {
            Name    = name;
            _policy = policy;
        }

        public ParallelNode Add(IBehaviourNode child)
        {
            _children.Add(child);
            return this;
        }

        public void OnEnter(Framework.StateMachine.StateContext ctx) { }

        public NodeStatus Tick(Framework.StateMachine.StateContext ctx)
        {
            if (_children.Count == 0) return NodeStatus.Success;

            int successCount = 0;
            int failureCount = 0;

            foreach (var child in _children)
            {
                var status = child.Tick(ctx);

                if (status == NodeStatus.Success) successCount++;
                if (status == NodeStatus.Failure) failureCount++;
            }

            switch (_policy)
            {
                case ParallelPolicy.RequireAll:
                    if (failureCount > 0)         return NodeStatus.Failure;
                    if (successCount == _children.Count)
                                                  return NodeStatus.Success;
                    return NodeStatus.Running;

                case ParallelPolicy.RequireOne:
                    if (successCount > 0)          return NodeStatus.Success;
                    if (failureCount == _children.Count)
                                                   return NodeStatus.Failure;
                    return NodeStatus.Running;

                default:
                    return NodeStatus.Running;
            }
        }

        public void OnExit(Framework.StateMachine.StateContext ctx,
                           NodeStatus status) { }
    }
}
