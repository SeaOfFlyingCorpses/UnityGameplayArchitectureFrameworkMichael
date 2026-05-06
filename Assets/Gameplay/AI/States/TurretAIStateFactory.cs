using UnityEngine;
using Framework.StateMachine;
using Framework.StateMachine.Conditions;
using Gameplay.Abilities;
using Gameplay.Abilities.Definitions;
using Framework.StateMachine.States;

namespace Gameplay.AI.States
{
    // =========================================
    // TurretAIStateFactory
    // Stationary agent — never moves.
    // Rotates toward target and fires.
    // Uses ranged ability (projectile) by default.
    //
    // Setup:
    //   Add component instead of CombatAIStateFactory
    //   Set Attack Range high (e.g. 15) for long range
    //   Assign a RangedAttack ability asset
    //   Remove Rigidbody or set constraints
    // =========================================
    public class TurretAIStateFactory : MonoBehaviour, IAIStateFactory
    {
        [Header("Combat")]
        public float attackCooldown = 2f;
        public float attackRange    = 15f;

        [Header("Abilities")]
        [Tooltip("Use a RangedAttack ability for projectiles")]
        public AbilityDefinition[] abilities;

        public IState Build(StateContext context)
        {
            var abilitySystem = context.Abilities as AbilitySystem;
            if (abilitySystem != null && abilities != null)
                foreach (var def in abilities)
                    if (def != null) abilitySystem.Register(def.Build());

            if (abilitySystem != null &&
                (abilities == null || abilities.Length == 0))
                abilitySystem.Register(
                    BasicAttackAbility.Create(
                        damage: 20, cooldown: attackCooldown));

            var config = new CombatStateConfig
            {
                AttackCooldown        = attackCooldown,
                IdealDistance         = attackRange, // stay at max range
                AttackRange           = attackRange,
                StrafeChance          = 0f,          // never strafe
                StrafeDecideInterval  = 99f,
                StrafeSpeedMultiplier = 0f
            };

            var idle    = new IdleState();
            var turret  = new TurretCombatState(config);
            var stagger = new StaggerState(idle);

            // Idle → Turret combat when target spotted
            idle.AddTransition(new Framework.StateMachine.Transition(
                new CanSeeTargetCondition(), turret));

            // Turret → Idle when target lost
            turret.AddTransition(new Framework.StateMachine.Transition(
                new TargetLostCondition(), idle));

            // Turret → Stagger on hit
            turret.AddTransition(new Framework.StateMachine.Transition(
                new WasHitCondition(), stagger));

            stagger.AddTransition(new Framework.StateMachine.Transition(
                new StaggerFinishedCondition(stagger), idle));

            return idle;
        }
    }
}