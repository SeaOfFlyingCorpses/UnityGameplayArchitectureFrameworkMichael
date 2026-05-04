using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine.Conditions;

namespace Framework.StateMachine.States
{
    public class StaggerState : IState
    {
        private readonly List<Transition> _transitions = new();

        private float _timer;
        private readonly float _duration;

        public StaggerState(IState returnState, float duration = 0.4f)
        {
            _duration = duration;

            _transitions.Add(new Transition(
                new StaggerFinishedCondition(this),
                returnState
            ));
        }

        public void Enter(StateContext context)
        {
            _timer = 0f;

            context.AnimationRequest = new AnimationRequest(AnimationType.Hit);

            context.Commands.Clear(); // will fix below
        }

        public void Update(StateContext context)
        {
            _timer += Time.deltaTime;
        }

        public void Exit() { }

        public List<Transition> GetTransitions() => _transitions;

        public bool IsFinished() => _timer >= _duration;
    }
}