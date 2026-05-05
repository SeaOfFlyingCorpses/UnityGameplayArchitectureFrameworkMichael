using System.Collections.Generic;

namespace Framework.StateMachine
{
    public interface IState
    {
        void Enter(StateContext context);
        void Update(StateContext context);
        void Exit();

        List<Transition> GetTransitions();

        // =========================================
        // AddTransition — wires transitions from
        // outside the state, keeping state logic
        // clean and factories in control of the graph
        // =========================================
        void AddTransition(Transition transition);
    }
}