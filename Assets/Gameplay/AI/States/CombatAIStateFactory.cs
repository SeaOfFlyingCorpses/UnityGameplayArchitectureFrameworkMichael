using UnityEngine;
using Framework.StateMachine;
using Framework.StateMachine.States;

namespace Gameplay.AI.States
{
    public class CombatAIStateFactory : MonoBehaviour, IAIStateFactory
    {
        [Header("Combat Tuning")]
        [Tooltip("Seconds between attacks")]
        public float attackCooldown = 1.2f;

        [Tooltip("Distance the agent tries to maintain from its target")]
        public float idealDistance  = 5.0f;

        [Tooltip("Distance at which the agent can land an attack — should be >= idealDistance")]
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

            var combat  = new CombatState(config);
            var idle    = new IdleState();
            var search  = new SearchState(idle);
            var chase   = new ChaseState(combat, search);
            var stagger = new StaggerState(combat);

            return combat;
        }
    }
}