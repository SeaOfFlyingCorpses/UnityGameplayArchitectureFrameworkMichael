using System.Collections.Generic;
using UnityEngine;
using Framework.StateMachine;
using Framework.AI.Faction;
using Framework.Core;
using Framework.StateMachine.States;
using Gameplay.AI;
using Gameplay.AI.Faction;
using Gameplay.AI.Threat;
using Gameplay.Combat;

namespace Gameplay.AI.States
{
    // =========================================
    // TurretCombatState
    // Rotates toward target and fires ability.
    // Never moves — use with TurretAIStateFactory.
    // =========================================
    public class TurretCombatState : IState
    {
        private readonly List<Transition>  _transitions = new();
        private readonly CombatStateConfig _config;
        private float _lastAttackTime;

        public TurretCombatState(CombatStateConfig config)
        {
            _config = config ?? new CombatStateConfig();
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context) { }

        public void Update(StateContext context)
        {
            var target = context.Target;

            if (target == null)
            {
                if (context.VisibleTargets == null ||
                    context.VisibleTargets.Count == 0) return;

                target = ThreatSystem.GetBestTarget(
                    context.VisibleTargets,
                    context.Self, context.Team);

                if (target == null) return;
                context.Target = target;
            }

            // Team check
            var registry = ServiceLocator.Get<AIAgentRegistry>();
            if (registry != null &&
                registry.TryGetContext(target, out var otherCtx))
                if (otherCtx != null &&
                    !TeamRelationship.IsHostile(context.Team, otherCtx.Team))
                    return;

            // Full 3D distance for range check
            float distance = Vector3.Distance(
                context.Self.position, target.position);

            // Rotate toward target on XZ plane only — no movement
            Vector3 dir = (target.position - context.Self.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                context.Self.rotation = Quaternion.Slerp(
                    context.Self.rotation,
                    Quaternion.LookRotation(dir.normalized),
                    Time.deltaTime * 5f);

            // Resolve target health
            Gameplay.Systems.Health.HealthComponent targetHealth = null;
            if (target.TryGetComponent(out Targetable targetable))
                targetHealth = targetable.Health;

            // Attack
            if (distance <= _config.AttackRange &&
                Time.time > _lastAttackTime + _config.AttackCooldown &&
                targetHealth != null)
            {
                var abilitySystem =
                    context.Abilities as Gameplay.Abilities.AbilitySystem;

                bool fired = abilitySystem?.UseBestAvailable(
                    new Gameplay.Abilities.AbilityContext
                    {
                        Self         = context.Self,
                        Target       = target,
                        SourceHealth = context.HealthComp,
                        TargetHealth = targetHealth
                    }) ?? false;

                if (fired) _lastAttackTime = Time.time;
            }
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}