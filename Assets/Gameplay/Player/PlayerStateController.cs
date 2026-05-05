using Framework.Commands;
using Framework.Input;
using Framework.StateMachine;
using Framework.StateMachine.States;
using Framework.StateMachine.Conditions;
using Framework.AI.Faction;
using Gameplay.Input;
using Gameplay.States;
using Gameplay.Systems.Health;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerStateController : MonoBehaviour
    {
        private StateMachine _stateMachine;
        private StateContext _context;

        [Header("References")]
        public InputHandler    inputHandler;
        public HealthComponent healthComponent;

        private void Awake()
        {
            _context = new StateContext
            {
                Commands   = new CommandQueue(),
                HealthData = healthComponent.GetHealth(),
                HealthComp = healthComponent,
                Self       = transform,
                Input      = inputHandler != null ? inputHandler.State : new InputState(),
                Team       = Team.Player
            };

            _stateMachine = new StateMachine(_context);
            _stateMachine.ChangeState(BuildStateGraph());
        }

        private void Update()
        {
            _stateMachine.Update();
            _context.Commands.ExecuteAll();
        }

        private IState BuildStateGraph()
        {
            var idle    = new IdleState();
            var move    = new MoveState();
            var attack  = new AttackState();
            var stagger = new StaggerState(idle);

            idle.AddTransition(new Transition(new MovePressedCondition(), move));
            idle.AddTransition(new Transition(new AttackPressedCondition(), attack));
            idle.AddTransition(new Transition(new WasHitCondition(), stagger));

            move.AddTransition(new Transition(new MoveReleasedCondition(), idle));
            move.AddTransition(new Transition(new AttackPressedCondition(), attack));
            move.AddTransition(new Transition(new WasHitCondition(), stagger));

            attack.Init(idle);

            stagger.AddTransition(new Transition(
                new StaggerFinishedCondition(stagger), idle));

            return idle;
        }
    }
}