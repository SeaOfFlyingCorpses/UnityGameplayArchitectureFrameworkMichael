using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;

namespace Gameplay.States
{
    // =========================================
    // LandState
    // Short state on landing — plays land animation
    // then immediately transitions to Idle or Move.
    // Duration keeps it from feeling floaty.
    // =========================================
    public class LandState : IState
    {
        private readonly List<Transition> _transitions = new();
        private float _timer;
        private readonly float _duration;

        public LandState(float duration = 0.1f)
        {
            _duration = duration;
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            _timer = 0f;
            context.AnimationRequest =
                new AnimationRequest(AnimationType.Land);
        }

        public void Update(StateContext context)
        {
            _timer += Time.deltaTime;
        }

        public bool IsFinished() => _timer >= _duration;

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}
