using UnityEngine;
using Framework.StateMachine;
using Gameplay.States;
using Gameplay.Systems.Movement;
using Gameplay.Abilities;
using Gameplay.Abilities.Definitions;
using Framework.StateMachine.States;
using Framework.StateMachine.Conditions;

namespace Gameplay.AI.States
{
    public class CombatAIStateFactory : MonoBehaviour, IAIStateFactory
    {
        [Header("2D Platformer")]
        [Tooltip("Use PlatformerChaseState instead of ChaseState.")]
        public bool      is2D          = false;
        public float     jumpThreshold = 1.5f;
        public LayerMask groundLayer;

        [Header("Abilities")]
        [Tooltip("Drag AbilityDefinition assets here. " +
                 "Agent will use whichever ability is off cooldown. " +
                 "Higher priority = preferred when multiple are ready.")]
        public AbilityDefinition[] abilities;

        [Header("Combat Tuning")]
        [Tooltip("How often the agent attempts to act")]
        public float attackCooldown = 1.2f;

        [Tooltip("Distance at which the agent can land an attack")]
        public float attackRange    = 5.5f;

        [Tooltip("Distance the agent tries to maintain from its target")]
        public float idealDistance  = 5.0f;

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
            var abilitySystem = context.Abilities as AbilitySystem;

            // Register all abilities from Inspector
            if (abilitySystem != null && abilities != null)
            {
                foreach (var def in abilities)
                {
                    if (def != null)
                        abilitySystem.Register(def.Build());
                }
            }

            // Fallback — no abilities assigned in Inspector
            if (abilitySystem != null &&
                (abilities == null || abilities.Length == 0))
            {
                abilitySystem.Register(
                    BasicAttackAbility.Create(
                        damage:   10,
                        cooldown: attackCooldown
                    ));
            }

            var config = new CombatStateConfig
            {
                AttackCooldown        = attackCooldown,
                IdealDistance         = idealDistance,
                AttackRange           = attackRange,
                StrafeChance          = strafeChance,
                StrafeDecideInterval  = strafeDecideInterval,
                StrafeSpeedMultiplier = strafeSpeedMultiplier
            };

            var idle    = new IdleState();
            var move    = new MoveState();
            var combat  = new CombatState(config);
            IState chase = is2D
                ? (IState) new PlatformerChaseState(
                    combat, idle, idealDistance, jumpThreshold)
                : new ChaseState(combat, idle);
            var stagger = new StaggerState(combat);

            idle.AddTransition(new Transition(
                new CanSeeTargetCondition(), chase));

            chase.AddTransition(new Transition(
                new IsInAttackRangeCondition(), combat));

            chase.AddTransition(new Transition(
                new TargetLostCondition(), idle));

            combat.AddTransition(new Transition(
                new TargetLostCondition(), chase));

            combat.AddTransition(new Transition(
                new WasHitCondition(), stagger));

            stagger.AddTransition(new Transition(
                new StaggerFinishedCondition(stagger), combat));

            return idle;
        }
    }
}