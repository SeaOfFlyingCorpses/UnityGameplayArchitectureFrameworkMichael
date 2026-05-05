using System.Collections.Generic;
using Framework.Animation;
using Framework.StateMachine;
using Framework.StateMachine.Conditions;
using UnityEngine;
using Gameplay.Systems.Health.Commands;
using Framework.Systems.Health;

namespace Gameplay.States
{
    public class AttackState : IState
    {
        private readonly List<Transition> _transitions = new();
        private float _timer;
        private float _duration = 0.4f;

        public void Init(IState idleState)
        {
            _transitions.Add(new Transition(
                new AttackFinishedCondition(() => _timer >= _duration), idleState));
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            _timer = 0f;
            context.AnimationRequest = new AnimationRequest(AnimationType.Attack);
            // HealthData is Framework.Systems.Health.IHealth — DamageCommand accepts it
            context.Commands.Enqueue(new DamageCommand(context.HealthData, 10));
        }

        public void Update(StateContext context) => _timer += Time.deltaTime;
        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}