using UnityEngine;
using Framework.StateMachine;
using Framework.StateMachine.States;
using Framework.StateMachine.Conditions;

namespace Gameplay.AI.States
{
    public class CombatAIStateFactory : MonoBehaviour, IAIStateFactory
    {
        [Header("Combat Tuning")]
        [Tooltip("Seconds between attacks")]
        public float attackCooldown = 1.2f;

        [Tooltip("Distance the agent tries to maintain from its target")]
        public float idealDistance  = 5.0f;

        [Tooltip("Distance at which the agent can land an attack")]
        public float attackRange    = 5.5f;

        [Header("Strafe Tuning")]
        [Tooltip("0 = never strafe  |  1 = always strafe")]
        [Range(0f, 1f)]
        public float strafeChance         = 0.4f;

        [Tooltip("Seconds between strafe decisions")]
        public float strafeDecideInterval = 2.5f;

        [Tooltip("Speed multiplier when strafing")]
        [Range(0.1f, 1f)]
        public float strafeSpeedMultiplier = 0.7f;

        public IState Build(StateContext context)
        {
            var config = new CombatStateConfig
            {
                AttackCooldown        = attackCooldown,
                IdealDistance         = idealDistance,
                AttackRange           = attackRange,
                StrafeChance          = strafeChance,
                StrafeDecideInterval  = strafeDecideInterval,
                StrafeSpeedMultiplier = strafeSpeedMultiplier
            };

            // =========================================
            // CREATE STATES
            // =========================================
            var idle    = new IdleState();
            var move    = new MoveState();
            var combat  = new CombatState(config);
            var chase   = new ChaseState(combat, idle);
            var stagger = new StaggerState(combat);

            // =========================================
            // WIRE TRANSITIONS
            //
            // IdleState
            //   → ChaseState  when target spotted
            //
            // ChaseState
            //   → CombatState when in attack range
            //   → IdleState   when target fully lost
            //
            // CombatState
            //   → ChaseState  when target moves out of range
            //   → StaggerState when hit
            //
            // StaggerState
            //   → CombatState when stagger finishes
            // =========================================

            idle.AddTransition(new Transition(
                new CanSeeTargetCondition(),
                chase
            ));

            chase.AddTransition(new Transition(
                new IsInAttackRangeCondition(),
                combat
            ));

            chase.AddTransition(new Transition(
                new TargetLostCondition(),
                idle
            ));

            combat.AddTransition(new Transition(
                new TargetLostCondition(),
                chase
            ));

            combat.AddTransition(new Transition(
                new WasHitCondition(),
                stagger
            ));

            stagger.AddTransition(new Transition(
                new StaggerFinishedCondition(stagger),
                combat
            ));

            // Start in idle — transitions to chase when target spotted
            return idle;
        }
    }
}