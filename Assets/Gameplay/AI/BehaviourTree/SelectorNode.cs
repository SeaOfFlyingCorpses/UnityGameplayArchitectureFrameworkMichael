using System.Collections.Generic;
using Framework.AI.BehaviourTree;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // SelectorNode  (OR gate)
    // Ticks children left to right.
    // Returns Success if ANY child succeeds.
    // Returns Failure only if ALL fail.
    // Returns Running while current child runs.
    //
    // Use for: "Try A, else try B, else try C"
    // Example: Attack → Chase → Patrol
    // =========================================
    public class SelectorNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly List<IBehaviourNode> _children = new();
        private int _current;

        public SelectorNode(string name = "Selector")
        {
            Name = name;
        }

        public SelectorNode Add(IBehaviourNode child)
        {
            _children.Add(child);
            return this;
        }

        public void OnEnter(Framework.StateMachine.StateContext ctx)
        {
            _current = 0;
        }

        public NodeStatus Tick(Framework.StateMachine.StateContext ctx)
        {
            if (_children.Count == 0) return NodeStatus.Failure;

            while (_current < _children.Count)
            {
                var status = _children[_current].Tick(ctx);

                switch (status)
                {
                    case NodeStatus.Success:
                        return NodeStatus.Success;

                    case NodeStatus.Running:
                        return NodeStatus.Running;

                    case NodeStatus.Failure:
                        _current++;
                        break;
                }
            }

            return NodeStatus.Failure;
        }

        public void OnExit(Framework.StateMachine.StateContext ctx,
                           NodeStatus status)
        {
            _current = 0;
        }
    }
}
