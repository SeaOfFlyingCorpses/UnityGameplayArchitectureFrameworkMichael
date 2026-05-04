using System.Collections.Generic;

namespace Framework.StateMachine
{
    public interface IState
    {
        void Enter(StateContext context);
        void Update(StateContext context);
        void Exit();

        List<Transition> GetTransitions();
    }
}