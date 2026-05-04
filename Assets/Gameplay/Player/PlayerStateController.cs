using Framework.Commands;
using Framework.Input;
using Framework.StateMachine;
using Framework.StateMachine.States;
using Gameplay.AI.Faction;
using Gameplay.Systems.Health;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerStateController : MonoBehaviour
    {
        private StateMachine _stateMachine;
        private StateContext _context;

        public InputHandler inputHandler;
        public HealthComponent healthComponent;

        [Header("Mode")]
        public bool UseAI = false; // toggle player vs AI

        private void Awake()
        {
            _context = new StateContext
            {
                Commands = new CommandQueue(),

                HealthData = healthComponent.GetHealth(),
                HealthComp = healthComponent,

                Self = transform,

                Input = inputHandler != null ? inputHandler.State : new InputState(),

                Team = Team.Player
            };

            _stateMachine = new StateMachine(_context);

            // create states
            var idle = new IdleState();
            var move = new MoveState();
            var attack = new AttackState();

            // link states
            idle.Init(move, attack);
            move.Init(idle, attack);
            attack.Init(idle);

            _stateMachine.ChangeState(idle);
        }

        private void Update()
        {
            // -----------------------------------
            // PLAYER MODE (INPUT DRIVEN)
            // -----------------------------------
            if (!UseAI)
            {
                // input already drives state system
                _stateMachine.Update();
                _context.Commands.ExecuteAll();
                return;
            }

            // -----------------------------------
            // AI MODE (COMMENTED FOR NOW)
            // -----------------------------------

            /*
            // AI perception update
            perception.Update();

            // AI systems
            MoralSystem.Update(_context);
            SuppressionSystem.Update(_context);
            Framework.AI.Alert.AlertSystem.Update(_context);

            // AI "fake input" injection (this replaces player input)
            if (_context.VisibleTargets != null && _context.VisibleTargets.Count > 0)
            {
                _context.Input.MoveHeld = true;
                _context.Input.AttackPressed = true;
            }
            else
            {
                _context.Input.MoveHeld = false;
                _context.Input.AttackPressed = false;
            }
            */

            // shared state machine still runs
            _stateMachine.Update();
            _context.Commands.ExecuteAll();
        }
    }
}