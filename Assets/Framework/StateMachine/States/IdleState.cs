using System.Collections.Generic;
using Framework.AI.Alert;
using Framework.Animation;
using Framework.StateMachine.Conditions;

namespace Framework.StateMachine.States
{
    public class IdleState : IState
    {
        private readonly List<Transition> _transitions = new();

        // link states after creation
        public void Init(IState moveState, IState attackState)
        {
            _transitions.Add(new Transition(
                new MovePressedCondition(),
                moveState
            ));

            _transitions.Add(new Transition(
                new AttackPressedCondition(),
                attackState
            ));
        }

        public void Enter(StateContext context)
        {
            context.AnimationRequest = new AnimationRequest(AnimationType.Idle);
        }

        public void Update(StateContext context)
        {
            context.AnimationRequest =
                new AnimationRequest(AnimationType.Idle);
        }

        public void Exit() { }

        public List<Transition> GetTransitions() => _transitions;
    }
}