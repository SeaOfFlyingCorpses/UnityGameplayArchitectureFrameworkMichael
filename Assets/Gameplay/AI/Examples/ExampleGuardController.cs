using Framework.AI.BehaviourTree;
using Gameplay.AI.BehaviourTree;
using Gameplay.Systems.Movement.Commands;
using UnityEngine;

namespace Gameplay.AI.Examples
{
    // =========================================
    // ExampleGuardController
    // Shows how to use BehaviourTreeController.
    // Copy and modify for your own agents.
    //
    // Behaviour:
    //   If health low   → flee
    //   Else if target visible AND in range → attack
    //   Else if target visible              → chase
    //   Else                                → patrol
    // =========================================
    public class ExampleGuardController : BehaviourTreeController
    {
        [Header("Guard Settings")]
        public float attackRange  = 2f;
        public float moveSpeed    = 3f;
        public float fleeSpeed    = 4f;
        public float lowHealthPct = 0.25f;

        protected override IBehaviourNode BuildTree()
        {
            // =========================================
            // FLEE BRANCH
            // =========================================
            var flee = new SequenceNode("Flee")
                .Add(new ConditionNode("HealthLow",
                    ctx => ctx.HealthData != null &&
                           (float)ctx.HealthData.Value /
                           ctx.HealthData.MaxValue < lowHealthPct))
                .Add(new ActionNode("FleeFromTarget",
                    ctx =>
                    {
                        if (ctx.Target == null) return;
                        Vector3 away = (ctx.Self.position -
                                       ctx.Target.position).normalized;
                        ctx.Commands.Enqueue(new MoveCommand(
                            ctx.Self, away, fleeSpeed, ctx.Movement));
                    }));

            // =========================================
            // COMBAT BRANCH
            // =========================================
            var attack = new SequenceNode("Attack")
                .Add(new ConditionNode("TargetVisible",
                    ctx => ctx.VisibleTargets?.Count > 0))
                .Add(new ConditionNode("TargetInRange",
                    ctx => ctx.Target != null &&
                           Vector3.Distance(ctx.Self.position,
                               ctx.Target.position) <= attackRange))
                .Add(new CooldownNode(
                    new ActionNode("DealDamage",
                        ctx =>
                        {
                            if (ctx.Target == null) return;
                            var health = ctx.Target
                                .GetComponent<Gameplay.Systems.Health
                                    .HealthComponent>();
                            health?.Damage(10, ctx.Self.position);
                        }),
                    cooldown: 1.5f,
                    name: "AttackCooldown"));

            // =========================================
            // CHASE BRANCH
            // =========================================
            var chase = new SequenceNode("Chase")
                .Add(new ConditionNode("TargetVisible",
                    ctx => ctx.VisibleTargets?.Count > 0))
                .Add(new ActionNode("MoveToTarget",
                    ctx =>
                    {
                        if (ctx.Target == null) return;
                        Vector3 dir = (ctx.Target.position -
                                      ctx.Self.position).normalized;
                        ctx.Commands.Enqueue(new MoveCommand(
                            ctx.Self, dir, moveSpeed, ctx.Movement));
                    }));

            // =========================================
            // PATROL BRANCH (fallback)
            // =========================================
            var patrol = new ActionNode("Idle",
                ctx => ctx.Movement?.Stop(ctx.Self));

            // =========================================
            // ROOT — try each branch in priority order
            // =========================================
            return new SelectorNode("Guard Root")
                .Add(flee)
                .Add(attack)
                .Add(chase)
                .Add(patrol);
        }
    }
}
