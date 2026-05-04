using System.Collections.Generic;
using UnityEngine;
using Framework.StateMachine;
using Gameplay.Abilities;
using Gameplay.AI.Squad;
using Gameplay.AI.Director;
using Gameplay.Systems.Movement.Commands;
using Gameplay.Systems.Health;
using Gameplay.AI;
using Gameplay.Combat;
using Gameplay.AI.Threat;

namespace Framework.StateMachine.States
{
    public class CombatState : IState
    {
        private readonly List<Transition> _transitions = new();

        private float _lastAttackTime;
        private float _attackCooldown = 1.2f;

        private float _idealDistance = 2.5f;

        private float _strafeTimer;
        private float _strafeDir = 1f;

        public void Enter(StateContext context) { }

        public void Update(StateContext context)
        {
            Transform target = context.Target;
            HealthComponent targetHealth = null;

            // ----------------------------
            // ✅ TARGET VALIDATION (NEW CORE)
            // ----------------------------
            if (target == null)
            {
                // 🔁 SAFE FALLBACK (DO NOT REMOVE YET)
                if (context.VisibleTargets == null || context.VisibleTargets.Count == 0)
                    return;

                target = ThreatSystem.GetBestTarget(
                    context.VisibleTargets,
                    context.Self
                );

                if (target == null)
                    return;

                context.Target = target; // sync back
            }

            // team check (safe)
            if (AIController.Registry.TryGetValue(target, out var otherCtx))
            {
                if (otherCtx != null && otherCtx.Team == context.Team)
                    return;
            }

            if (target.TryGetComponent(out Targetable targetable))
                targetHealth = targetable.Health;

            // ----------------------------
            // GLOBAL MODIFIERS
            // ----------------------------
            float intensity = AIDirector.Instance != null
                ? AIDirector.Instance.State.Intensity
                : 0f;

            float suppressionFactor = 1f - context.Suppression;

            // ----------------------------
            // FEAR BEHAVIOR
            // ----------------------------
            if (context.Fear > 0.8f)
            {
                Vector3 away = (context.Self.position - target.position).normalized;

                context.Commands.Enqueue(
                    new MoveCommand(context.Self, away, 6f * suppressionFactor)
                );
                return;
            }

            // ----------------------------
            // MORALE RETREAT
            // ----------------------------
            if (context.Morale < 0.3f)
            {
                context.Commands.Enqueue(
                    new MoveCommand(context.Self, -context.Self.forward, 3f * suppressionFactor)
                );
                return;
            }

            // ----------------------------
            // SQUAD RETREAT
            // ----------------------------
            var squad = SquadSystem.Instance?.GlobalSquad;

            if (squad != null &&
                squad.CurrentStrategy == SquadStrategy.Retreat)
            {
                Vector3 away = (context.Self.position - target.position).normalized;

                context.Commands.Enqueue(
                    new MoveCommand(context.Self, away, 5f * suppressionFactor)
                );
                return;
            }

            // ----------------------------
            // MOVEMENT
            // ----------------------------
            Vector3 dir = target.position - context.Self.position;
            float distance = dir.magnitude;

            float speed = 3.5f * suppressionFactor;
            speed *= Mathf.Lerp(0.8f, 1.5f, intensity);

            _strafeTimer += Time.deltaTime;
            if (_strafeTimer > 2f)
            {
                _strafeDir *= -1f;
                _strafeTimer = 0f;
            }

            Vector3 forward = dir.normalized;
            Vector3 strafe = Vector3.Cross(Vector3.up, forward) * _strafeDir;

            if (distance < _idealDistance - 0.5f)
            {
                context.Commands.Enqueue(new MoveCommand(context.Self, -forward, speed));
                return;
            }

            if (distance <= _idealDistance + 1f)
            {
                context.Commands.Enqueue(new MoveCommand(context.Self, strafe, speed));
            }
            else
            {
                context.Commands.Enqueue(new MoveCommand(context.Self, forward, speed));
            }

            // ----------------------------
            // ATTACK
            // ----------------------------
            float cooldown = _attackCooldown + context.Suppression;

            if (distance <= _idealDistance)
            {
                if (Time.time > _lastAttackTime + cooldown)
                {
                    if (targetHealth != null)
                    {
                        context.Abilities.Use("Attack", new AbilityContext
                        {
                            Self = context.Self,
                            Target = target,
                            SourceHealth = context.HealthComp,
                            TargetHealth = targetHealth
                        });
                    }

                    _lastAttackTime = Time.time;
                }
            }
        }

        public void Exit() { }

        public List<Transition> GetTransitions() => _transitions;
    }
}