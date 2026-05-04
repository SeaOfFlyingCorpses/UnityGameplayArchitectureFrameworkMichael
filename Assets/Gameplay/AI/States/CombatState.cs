using System.Collections.Generic;
using UnityEngine;
using Gameplay.AI.Squad;
using Gameplay.AI.Faction;
using Gameplay.Systems.Movement.Commands;
using Gameplay.Abilities;
using Gameplay.AI;
using Gameplay.Combat;
using Gameplay.AI.Threat;

namespace Framework.StateMachine.States
{
    public class CombatStateConfig
    {
        public float AttackCooldown        = 1.2f;
        public float IdealDistance         = 5.0f;
        public float AttackRange           = 5.5f;
        public float StrafeChance          = 0.4f;
        public float StrafeDecideInterval  = 2.5f;
        public float StrafeSpeedMultiplier = 0.7f;
    }

    public class CombatState : IState
    {
        private readonly List<Transition>  _transitions = new();
        private readonly CombatStateConfig _config;

        private float _lastAttackTime;
        private float _strafeTimer;
        private float _strafeDir   = 1f;
        private bool  _isStrafeFrame;

        public CombatState() : this(new CombatStateConfig()) { }

        public CombatState(CombatStateConfig config)
        {
            _config = config ?? new CombatStateConfig();
        }

        public void Enter(StateContext context)
        {
            _strafeTimer   = Random.Range(0f, _config.StrafeDecideInterval);
            _isStrafeFrame = Random.value < _config.StrafeChance;
        }

        public void Update(StateContext context)
        {
            // ----------------------------
            // TARGET VALIDATION
            // ----------------------------
            var target = context.Target;

            if (target == null)
            {
                if (context.VisibleTargets == null || context.VisibleTargets.Count == 0)
                    return;

                target = ThreatSystem.GetBestTarget(
                    context.VisibleTargets,
                    context.Self,
                    context.Team
                );

                if (target == null)
                    return;

                context.Target = target;
            }

            // team check
            if (AIController.Registry.TryGetValue(target, out var otherCtx))
            {
                if (otherCtx != null && !TeamRelationship.IsHostile(context.Team, otherCtx.Team))
                    return;
            }

            // resolve target health
            Gameplay.Systems.Health.HealthComponent targetHealth = null;
            if (target.TryGetComponent(out Targetable targetable))
                targetHealth = targetable.Health;

            // ----------------------------
            // GLOBAL MODIFIERS
            // ----------------------------
            float intensity   = context.DirectorIntensity;
            float suppression = 1f - context.Suppression;

            // ----------------------------
            // FEAR BEHAVIOR
            // ----------------------------
            if (context.Fear > 0.8f)
            {
                var away = (context.Self.position - target.position).normalized;
                context.Commands.Enqueue(new MoveCommand(context.Self, away, 6f * suppression));
                return;
            }

            // ----------------------------
            // MORALE RETREAT
            // ----------------------------
            if (context.Morale < 0.3f)
            {
                context.Commands.Enqueue(
                    new MoveCommand(context.Self, -context.Self.forward, 3f * suppression));
                return;
            }

            // ----------------------------
            // SQUAD RETREAT
            // ----------------------------
            if (context.SquadStrategy == SquadStrategy.Retreat)
            {
                var away = (context.Self.position - target.position).normalized;
                context.Commands.Enqueue(new MoveCommand(context.Self, away, 5f * suppression));
                return;
            }

            // ----------------------------
            // STRAFE DECISION
            // ----------------------------
            _strafeTimer += Time.deltaTime;
            if (_strafeTimer >= _config.StrafeDecideInterval)
            {
                _strafeTimer   = 0f;
                _strafeDir    *= -1f;
                _isStrafeFrame = Random.value < _config.StrafeChance;
            }

            // ----------------------------
            // MOVEMENT
            // ----------------------------
            var   dir      = target.position - context.Self.position;
            float distance = dir.magnitude;
            float speed    = 3.5f * suppression * Mathf.Lerp(0.8f, 1.5f, intensity);

            var forward = dir.normalized;
            var strafe  = Vector3.Cross(Vector3.up, forward) * _strafeDir;

            if (distance < _config.IdealDistance - 0.5f)
            {
                context.Commands.Enqueue(new MoveCommand(context.Self, -forward, speed));
            }
            else if (distance <= _config.IdealDistance + 1f)
            {
                if (_isStrafeFrame)
                    context.Commands.Enqueue(
                        new MoveCommand(context.Self, strafe, speed * _config.StrafeSpeedMultiplier));
            }
            else
            {
                context.Commands.Enqueue(new MoveCommand(context.Self, forward, speed));
            }

            // ----------------------------
            // ATTACK
            // ----------------------------
            if (distance <= _config.AttackRange &&
                Time.time > _lastAttackTime + _config.AttackCooldown + context.Suppression &&
                targetHealth != null)
            {
                context.Abilities?.Use("Attack", new AbilityContext
                {
                    Self         = context.Self,
                    Target       = target,
                    SourceHealth = context.HealthComp,
                    TargetHealth = targetHealth
                });

                _lastAttackTime = Time.time;
            }
        }

        public void Exit() { }

        public List<Transition> GetTransitions() => _transitions;
    }
}