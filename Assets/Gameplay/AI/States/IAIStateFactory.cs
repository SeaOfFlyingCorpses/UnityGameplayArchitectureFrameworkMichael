using Framework.StateMachine;

namespace Gameplay.AI.States
{
    // =========================================
    // IAIStateFactory
    // Builds a state graph and returns the
    // initial state for an AI agent.
    //
    // Implement this to define a custom AI
    // behaviour profile without touching AIController.
    //
    // Examples:
    //   CombatAIStateFactory  — full combat soldier
    //   PatrolAIStateFactory  — waypoint patroller
    //   TurretAIStateFactory  — stationary attacker
    //   CivilianAIStateFactory — flee-only NPC
    // =========================================
    public interface IAIStateFactory
    {
        IState Build(StateContext context);
    }
}