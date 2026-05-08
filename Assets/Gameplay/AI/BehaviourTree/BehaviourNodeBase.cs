using Framework.AI.BehaviourTree;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // BehaviourNodeBase
    // Abstract base — handles enter/exit state.
    // Subclass for all custom leaf nodes.
    //
    // Override:
    //   Execute() — your node logic each frame
    //   Enter()   — optional setup on activate
    //   Exit()    — optional cleanup on complete
    // =========================================
    public abstract class BehaviourNodeBase : IBehaviourNode
    {
        public abstract string Name { get; }

        private bool _entered;

        public NodeStatus Tick(Framework.StateMachine.StateContext context)
        {
            if (!_entered)
            {
                OnEnter(context);
                Enter(context);
                _entered = true;
            }

            var status = Execute(context);

            if (status != NodeStatus.Running)
            {
                OnExit(context, status);
                Exit(context, status);
                _entered = false;
            }

            return status;
        }

        public void OnEnter(Framework.StateMachine.StateContext ctx) { }
        public void OnExit (Framework.StateMachine.StateContext ctx,
                            NodeStatus status) { }

        // =========================================
        // OVERRIDE IN SUBCLASS
        // =========================================
        protected virtual void       Enter  (Framework.StateMachine.StateContext ctx) { }
        protected abstract NodeStatus Execute(Framework.StateMachine.StateContext ctx);
        protected virtual void       Exit   (Framework.StateMachine.StateContext ctx,
                                             NodeStatus status) { }
    }
}
