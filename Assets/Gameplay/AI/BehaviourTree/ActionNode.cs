using System;
using Framework.AI.BehaviourTree;
using Framework.StateMachine;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // ActionNode
    // Executes a one-shot action.
    // Always returns Success after executing.
    //
    // Usage:
    //   new ActionNode("StopMoving",
    //       ctx => ctx.Movement?.Stop(ctx.Self))
    //
    //   new ActionNode("SetAlert",
    //       ctx => ctx.AlertContext.Level =
    //           Framework.AI.Alert.AlertLevel.Combat)
    // =========================================
    public class ActionNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly Action<StateContext> _action;

        public ActionNode(
            string               name,
            Action<StateContext> action)
        {
            Name    = name;
            _action = action;
        }

        public void OnEnter(StateContext ctx) { }

        public NodeStatus Tick(StateContext ctx)
        {
            _action?.Invoke(ctx);
            return NodeStatus.Success;
        }

        public void OnExit(StateContext ctx, NodeStatus status) { }
    }
}
