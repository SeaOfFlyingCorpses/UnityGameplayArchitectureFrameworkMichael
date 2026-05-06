using UnityEngine;
using Framework.StateMachine;
using Framework.StateMachine.Conditions;
using Gameplay.Abilities;
using Gameplay.Abilities.Definitions;
using Gameplay.States;
using Gameplay.Systems.Movement;
using Framework.StateMachine.States;

namespace Gameplay.AI.States
{
    // =========================================
    // PatrolAIStateFactory
    // Agent patrols waypoints or wanders randomly.
    // Transitions to combat when target spotted.
    // Returns to patrol when target lost.
    //
    // Setup — waypoint patrol:
    //   Add component to enemy
    //   Drag patrol point Transforms into Waypoints
    //
    // Setup — random wander:
    //   Leave Waypoints empty
    //   Set Wander Radius
    // =========================================
    public class PatrolAIStateFactory : MonoBehaviour, IAIStateFactory
    {
        [Header("Patrol")]
        [Tooltip("Leave empty for random wander")]
        public Transform[] waypoints;
        public float       patrolSpeed   = 2.5f;
        public float       waitAtPoint   = 1.5f;
        public float       wanderRadius  = 8f;

        [Header("2D Platformer")]
        [Tooltip("Use PlatformerChaseState instead of ChaseState. " +
                 "Enable for 2D platformer agents.")]
        public bool        is2D              = false;
        public float       jumpThreshold     = 1.5f;
        [Tooltip("Ground layer for ledge detection")]
        public LayerMask   groundLayer;

        [Header("Combat Tuning")]
        public float attackCooldown = 1.2f;
        public float attackRange    = 5.5f;
        public float idealDistance  = 5.0f;

        [Header("Strafe Tuning")]
        [Range(0f, 1f)]
        public float strafeChance         = 0.3f;
        public float strafeDecideInterval = 2.5f;
        [Range(0.1f, 1f)]
        public float strafeSpeedMultiplier = 0.7f;

        [Header("Abilities")]
        public AbilityDefinition[] abilities;

        public IState Build(StateContext context)
        {
            // Register abilities
            var abilitySystem = context.Abilities as AbilitySystem;
            if (abilitySystem != null && abilities != null)
                foreach (var def in abilities)
                    if (def != null) abilitySystem.Register(def.Build());

            if (abilitySystem != null &&
                (abilities == null || abilities.Length == 0))
                abilitySystem.Register(
                    BasicAttackAbility.Create(
                        cooldown: attackCooldown));

            // Create states
            var patrol  = waypoints != null && waypoints.Length > 0
                ? new PatrolState(waypoints, patrolSpeed, waitAtPoint)
                : new PatrolState(patrolSpeed, waitAtPoint, wanderRadius);

            var config  = new CombatStateConfig
            {
                AttackCooldown        = attackCooldown,
                IdealDistance         = idealDistance,
                AttackRange           = attackRange,
                StrafeChance          = strafeChance,
                StrafeDecideInterval  = strafeDecideInterval,
                StrafeSpeedMultiplier = strafeSpeedMultiplier
            };

            var combat  = new CombatState(config);
            IState chase = is2D
                ? (IState) new PlatformerChaseState(
                    combat, patrol, patrolSpeed, jumpThreshold)
                : new ChaseState(combat, patrol);
            var stagger = new StaggerState(combat);

            // Patrol → Chase when target spotted
            patrol.AddTransition(new Framework.StateMachine.Transition(
                new CanSeeTargetCondition(), chase));

            // Chase → Combat when in range
            chase.AddTransition(new Framework.StateMachine.Transition(
                new IsInAttackRangeCondition(), combat));

            // Chase → Patrol when target lost
            chase.AddTransition(new Framework.StateMachine.Transition(
                new TargetLostCondition(), patrol));

            // Combat → Chase when target out of range
            combat.AddTransition(new Framework.StateMachine.Transition(
                new TargetLostCondition(), chase));

            // Combat → Stagger on hit
            combat.AddTransition(new Framework.StateMachine.Transition(
                new WasHitCondition(), stagger));

            // Stagger → Combat when finished
            stagger.AddTransition(new Framework.StateMachine.Transition(
                new StaggerFinishedCondition(stagger), combat));

            return patrol;
        }
    }
}