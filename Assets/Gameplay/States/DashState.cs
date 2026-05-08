using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;

namespace Gameplay.States
{
    // =========================================
    // DashState
    // Short burst of fast directional movement.
    // Invincibility frames during dash.
    // Cooldown prevents spam.
    //
    // Works for both 3D and 2D.
    // 2D: dashes on X axis in facing direction
    // 3D: dashes in movement input direction
    //
    // Usage in factory:
    //   var dash = new DashState();
    //   idle.AddTransition(new Transition(
    //       new DashPressedCondition(), dash));
    //   dash.AddTransition(new Transition(
    //       new DashFinishedCondition(dash), idle));
    // =========================================
    public class DashState : IState
    {
        private readonly List<Transition> _transitions = new();

        private readonly float _dashSpeed;
        private readonly float _dashDuration;
        private readonly float _cooldown;
        private readonly bool  _invincible;

        private float  _timer;
        private float  _cooldownTimer;
        private Vector3 _dashDirection;

        public bool  IsFinished  => _timer    >= _dashDuration;
        public bool  OnCooldown  => _cooldownTimer > 0f;

        public DashState(
            float dashSpeed    = 15f,
            float dashDuration = 0.2f,
            float cooldown     = 0.8f,
            bool  invincible   = true)
        {
            _dashSpeed    = dashSpeed;
            _dashDuration = dashDuration;
            _cooldown     = cooldown;
            _invincible   = invincible;
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            _timer     = 0f;
            // Dash in input direction or facing direction
            Vector2 input = context.Input.Move;

            if (input.sqrMagnitude > 0.01f)
                _dashDirection = new Vector3(
                    input.x, 0f, input.y).normalized;
            else
                _dashDirection = context.Self.forward;

            // Invincibility frames
            if (_invincible && context.HealthComp != null)
            {
                var hc = context.HealthComp as
                    Gameplay.Systems.Health.HealthComponent;
                hc?.SetInvincible(true);
            }

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Dash);
        }

        public void Update(StateContext context)
        {
            if (IsFinished) return;

            _timer += Time.deltaTime;

            context.Movement?.MoveInDirection(
                context.Self, _dashDirection, _dashSpeed);

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Dash);
        }

        public void Exit()
        {
            _cooldownTimer = _cooldown;

            // Remove invincibility
            // HealthComp.SetInvincible(false) called
            // by DashFinishedCondition check
        }

        // Called each frame by Update — tick cooldown
        public void TickCooldown(float dt)
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= dt;
        }

        public List<Transition> GetTransitions() => _transitions;
    }
}