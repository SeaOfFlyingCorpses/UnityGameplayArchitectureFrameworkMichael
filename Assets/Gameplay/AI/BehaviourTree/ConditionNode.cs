using System;
using Framework.AI.BehaviourTree;
using Framework.StateMachine;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // ConditionNode
    // Evaluates a predicate against StateContext.
    // Returns Success if true, Failure if false.
    //
    // Usage:
    //   new ConditionNode("CanSeeTarget",
    //       ctx => ctx.VisibleTargets?.Count > 0)
    //
    //   new ConditionNode("IsHealthLow",
    //       ctx => ctx.HealthData?.Value < 30)
    // =========================================
    public class ConditionNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly Func<StateContext, bool> _condition;

        public ConditionNode(
            string                    name,
            Func<StateContext, bool>  condition)
        {
            Name       = name;
            _condition = condition;
        }

        public void OnEnter(StateContext ctx) { }

        public NodeStatus Tick(StateContext ctx)
        {
            return _condition(ctx)
                ? NodeStatus.Success
                : NodeStatus.Failure;
        }

        public void OnExit(StateContext ctx, NodeStatus status) { }
    }
}
