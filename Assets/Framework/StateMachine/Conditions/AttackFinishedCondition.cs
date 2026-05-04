using System;

namespace Framework.StateMachine.Conditions
{
    public class AttackFinishedCondition : ITransitionCondition
    {
        private readonly Func<bool> _finished;

        public AttackFinishedCondition(Func<bool> finished)
        {
            _finished = finished;
        }

        public bool Evaluate(StateContext context)
        {
            return _finished();
        }
    }
}