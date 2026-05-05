using System.Collections.Generic;
using Framework.Animation;
using Framework.StateMachine;
using UnityEngine;
using Framework.StateMachine.Conditions;
using Gameplay.Systems.Movement.Commands;

namespace Gameplay.States
{
    // Moved to Gameplay — uses MoveCommand
    public class MoveState : IState
    {
        private readonly List<Transition> _transitions = new();

        public void Init(IState idleState, IState attackState)
        {
            _transitions.Add(new Transition(new MoveReleasedCondition(), idleState));
            _transitions.Add(new Transition(new AttackPressedCondition(), attackState));
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            context.AnimationRequest = new AnimationRequest(AnimationType.Move);
        }

        public void Update(StateContext context)
        {
            Vector2 input = context.Input.Move;
            if (input.sqrMagnitude > 0.01f)
            {
                Vector3 moveDir = new Vector3(input.x, 0f, input.y);
                context.Commands.Enqueue(
                    new MoveCommand(context.Self, moveDir.normalized, 5f));
            }
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}
