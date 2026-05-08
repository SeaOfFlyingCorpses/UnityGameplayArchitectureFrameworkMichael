using System.Collections.Generic;
using Framework.AI.BehaviourTree;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // SequenceNode  (AND gate)
    // Ticks children left to right.
    // Returns Failure if ANY child fails.
    // Returns Success only if ALL succeed.
    // Returns Running while current child runs.
    //
    // Use for: "Do A, then B, then C"
    // Example: MoveToTarget → Attack → Retreat
    // =========================================
    public class SequenceNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly List<IBehaviourNode> _children = new();
        private int _current;

        public SequenceNode(string name = "Sequence")
        {
            Name = name;
        }

        public SequenceNode Add(IBehaviourNode child)
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
            if (_children.Count == 0) return NodeStatus.Success;

            while (_current < _children.Count)
            {
                var status = _children[_current].Tick(ctx);

                switch (status)
                {
                    case NodeStatus.Failure:
                        return NodeStatus.Failure;

                    case NodeStatus.Running:
                        return NodeStatus.Running;

                    case NodeStatus.Success:
                        _current++;
                        break;
                }
            }

            return NodeStatus.Success;
        }

        public void OnExit(Framework.StateMachine.StateContext ctx,
                           NodeStatus status)
        {
            _current = 0;
        }
    }
}
