using Framework.AI.BehaviourTree;
using Framework.StateMachine;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // BehaviourTree
    // Runs a root node each frame.
    // Drop-in replacement for StateMachine.
    //
    // Usage:
    //   var tree = new BehaviourTree(rootNode);
    //   tree.Tick(context);
    // =========================================
    public class BehaviourTree
    {
        private readonly IBehaviourNode _root;
        private          NodeStatus     _lastStatus;

        public NodeStatus LastStatus => _lastStatus;

        public BehaviourTree(IBehaviourNode root)
        {
            _root = root;
        }

        public void Tick(StateContext context)
        {
            if (_root == null) return;
            _lastStatus = _root.Tick(context);
        }
    }
}
