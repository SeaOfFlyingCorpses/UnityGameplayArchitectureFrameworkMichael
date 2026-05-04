using UnityEngine;
using Framework.Input;
using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    public class MoveReleasedCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
        {
            return context.Input.Move.sqrMagnitude <= 0.01f;
        }
    }
}