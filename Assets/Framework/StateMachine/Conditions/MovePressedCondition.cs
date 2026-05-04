using Framework.Input;
using Framework.StateMachine;
using UnityEngine;

namespace Framework.StateMachine.Conditions
{
    public class MovePressedCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
        {
            return context.Input.Move != Vector2.zero;
        }
    }
}