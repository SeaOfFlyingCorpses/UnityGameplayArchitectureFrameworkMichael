using System.Collections.Generic;
using Framework.Animation;
using UnityEngine;
using Framework.StateMachine.Conditions;
using Gameplay.Systems.Health.Commands;

namespace Framework.StateMachine.States
{
    public class AttackState : IState
    {
        private readonly List<Transition> _transitions = new();

        private float _timer;
        private float _duration = 0.4f;

        public void Init(IState idleState)
        {
            _transitions.Add(new Transition(
                new AttackFinishedCondition(() => _timer >= _duration),
                idleState
            ));
        }

        public void AddTransition(Transition transition) => _transitions.Add(transition);

        public void Enter(StateContext context)
        {
            _timer = 0f;
            context.AnimationRequest = new AnimationRequest(AnimationType.Attack);
            context.Commands.Enqueue(new DamageCommand(context.HealthData, 10));
        }

        public void Update(StateContext context)
        {
            _timer += Time.deltaTime;
        }

        public void Exit() { }

        public List<Transition> GetTransitions() => _transitions;
    }
}