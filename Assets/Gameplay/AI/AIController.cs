using System.Collections.Generic;
using UnityEngine;

using Framework.StateMachine;
using Framework.StateMachine.States;
using Framework.Commands;
using Framework.Input;
using Framework.AI.Systems;
using Framework.AI.Squad;
using Framework.AI.Lifecycle;
using Framework.Core;
using Framework.Animation;

using Gameplay.AI.Perception;
using Gameplay.AI.Group;
using Gameplay.AI.Squad;
using Gameplay.AI.Director;
using Gameplay.AI.Memory;
using Gameplay.AI.Faction;
using Framework.AI.Faction;
using Gameplay.AI.States;

using Gameplay.Systems.Health;
using Gameplay.Abilities;
using Gameplay.Combat;
using Gameplay.AI.Systems;
using Gameplay.States;
using Gameplay.Systems.Movement;
using UnityEngine.AI;

namespace Gameplay.AI
{
    public class AIController : MonoBehaviour, IAIEntity
    {
        private StateMachine    _stateMachine;
        private StateContext    _context;
        private AISystemManager _aiSystems;

        // =========================================
        // _initialized prevents pool pre-warm from
        // registering agents before context exists.
        // Pool deactivates prefab before Instantiate
        // so Awake/OnEnable never fire during pre-warm.
        // Start() fires after Awake — safe to register.
        // OnEnable fires when pool returns agent — safe
        // because _initialized is already true.
        // =========================================
        private bool _initialized;

        [Header("References")]
        public PerceptionSystem perception;
        public HealthComponent  healthComponent;
        public Rigidbody        rb;

        [Header("Player Detection")]
        [Tooltip("Tag used to find the player at runtime")]
        public string playerTag = "Player";

        private Transform _playerTransform;

        [Header("2D Settings")]
        [Tooltip("Ground layer for ledge detection (2D platformer only)")]
        public LayerMask groundLayer;

        [Header("AI Settings")]
        public Team team;

        protected IAIStateFactory StateFactory;

        // =========================================
        // UNITY LIFECYCLE
        // =========================================
        private void Awake()  => Initialize();
        private void Start()  => RegisterSquad();
        private void OnDestroy() => Dispose();

        private void OnEnable()
        {
            // Fires when pool activates this agent after Get()
            // _initialized is true at this point so register safely
            if (_initialized)
                ServiceLocator.Get<SquadSystem>()?.Register(_context);
        }

        private void OnDisable()
        {
            if (_initialized)
                ServiceLocator.Get<SquadSystem>()?.Unregister(_context);
        }

        private void Update()
        {
            TickMovement();
            TickPerception();
            TickAISystems();
            TickStateMachine();
            ResetFrameFlags();
            ExecuteCommands();
        }

        // =========================================
        // IAIEntity
        // =========================================
        public void Initialize()
        {
            if (_initialized) return;

            // Find player by tag at runtime
            // Avoids scene-reference issues on prefabs
            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
                _playerTransform = playerObj.transform;

            CreateContext();
            RegisterSelf();
            BindSystems();
            BindEvents();
            CreateStateMachine();
            RegisterDirector();

            _initialized = true;
            // Squad registration handled by Start()
        }

        public void OnDeath() => Cleanup();
        public void Dispose() => Cleanup();

        // =========================================
        // SQUAD REGISTRATION
        // Called from Start() — fires after Awake
        // so _initialized is guaranteed true
        // =========================================
        private void RegisterSquad()
        {
            ServiceLocator.Get<SquadSystem>()?.Register(_context);
        }

        // =========================================
        // INITIALIZATION
        // =========================================
        private void CreateContext()
        {
            _context = new StateContext
            {
                Input    = new InputState(),
                Commands = new CommandQueue(),

                HealthData = healthComponent.GetHealth(),
                HealthComp = healthComponent,

                Self = transform,

                PerceptionContext = new Gameplay.AI.Perception.PerceptionContext
                {
                    State          = new PerceptionState(),
                    Target         = _playerTransform,
                    VisibleTargets = new List<Transform>()
                },

                MemoryContext = new Gameplay.AI.Memory.MemoryContext(),
                AlertContext  = new Framework.AI.Alert.AlertContext(),
                SquadContext  = new Gameplay.AI.Squad.SquadContext(),

                Team = team
            };
        }

        private void RegisterSelf()
        {
            ServiceLocator.Get<AIAgentRegistry>()?.Register(transform, _context);
        }

        private void BindSystems()
        {
            _aiSystems = new AISystemManager();
            AISystemsBootstrap.RegisterDefaults(_aiSystems);

            var groupManager = ServiceLocator.Get<AIGroupManager>();
            if (groupManager != null)
            {
                _aiSystems.Register(new AIGroupSystem(groupManager));
                groupManager.Register(_context);
            }

            var stagger = GetComponent<StaggerSystem>();
            if (stagger != null && rb != null)
                stagger.Register(healthComponent, rb);

            var death = GetComponent<DeathSystem>();
            if (death != null)
                death.Register(healthComponent, rb);

            // Abilities — factory registers them in Build()
            _context.Abilities = new AbilitySystem();

            if (perception != null)
            {
                perception.context = _context;
                perception.target  = _playerTransform;
            }

            var animSystem = GetComponent<AnimationSystem>();
            if (animSystem != null)
                animSystem.SetContext(_context);

            // Auto-detect movement strategy
            var navAgent = GetComponent<NavMeshAgent>();
            var rb2d     = GetComponent<Rigidbody2D>();

            if (navAgent != null)
                _context.Movement = new NavMeshMovementStrategy(navAgent);
            else if (rb2d != null && rb2d.gravityScale == 0f)
                _context.Movement = new TopDown2DMovementStrategy(rb2d);
            else if (rb2d != null)
            {
                _context.Movement = new PlatformerMovementStrategy(rb2d);
                // Auto-register 2D systems for platformer agents
                AISystemsBootstrap.Register2D(
                    _aiSystems,
                    groundLayer);
            }
            else
                _context.Movement = new TransformMovementStrategy();
        }

        private void BindEvents()
        {
            if (healthComponent == null) return;
            healthComponent.OnHit   += OnHit;
            healthComponent.OnDeath += OnDeath;
        }

        private void CreateStateMachine()
        {
            if (StateFactory == null)
                StateFactory = CreateFactory();

            _stateMachine = new StateMachine(_context);
            _stateMachine.ChangeState(StateFactory.Build(_context));
        }

        protected virtual IAIStateFactory CreateFactory()
        {
            // GetComponent<IAIStateFactory> finds ANY factory —
            // CombatAIStateFactory, PatrolAIStateFactory,
            // TurretAIStateFactory, or any custom factory
            var factory = GetComponent<IAIStateFactory>();
            if (factory != null) return factory;
            return new DefaultCombatFactory();
        }

        private void RegisterDirector()
        {
            var director = ServiceLocator.Get<AIDirector>();
            if (director != null)
                director.State.ActiveEnemies++;
        }

        // =========================================
        // UPDATE
        // =========================================
        private void TickMovement()     => _context.Movement?.Tick(_context);
        private void TickPerception()   => perception?.Tick();
        private void TickAISystems()    => _aiSystems?.UpdateAll(_context);
        private void TickStateMachine() => _stateMachine?.Update();
        private void ResetFrameFlags()  => _context.WasHit = false;
        private void ExecuteCommands()  => _context.Commands?.ExecuteAll();

        // =========================================
        // CLEANUP
        // =========================================
        private void Cleanup()
        {
            _initialized = false;

            ServiceLocator.Get<AIAgentRegistry>()?.Unregister(transform);
            ServiceLocator.Get<AIGroupManager>()?.Unregister(_context);

            var director = ServiceLocator.Get<AIDirector>();
            if (director != null)
                director.State.ActiveEnemies--;

            if (healthComponent != null)
            {
                healthComponent.OnHit   -= OnHit;
                healthComponent.OnDeath -= OnDeath;
            }

            _aiSystems?.Clear();
            _aiSystems = null;

            if (_context != null)
                _context.Commands = null;

            enabled = false;
        }

        private void OnHit(Vector3 hitPoint)
        {
            _context.WasHit       = true;
            _context.HitDirection = (transform.position - hitPoint).normalized;
        }

        // =========================================
        // DEFAULT FACTORY — used when no factory
        // component is on the agent
        // =========================================
        private class DefaultCombatFactory : IAIStateFactory
        {
            public IState Build(StateContext context)
            {
                var config  = new CombatStateConfig();
                var combat  = new CombatState(config);
                var idle    = new IdleState();
                var search  = new SearchState(idle);
                var chase   = new ChaseState(combat, search);
                var stagger = new StaggerState(combat);

                idle.AddTransition(new Framework.StateMachine.Transition(
                    new Framework.StateMachine.Conditions.CanSeeTargetCondition(), chase));
                chase.AddTransition(new Framework.StateMachine.Transition(
                    new Framework.StateMachine.Conditions.IsInAttackRangeCondition(), combat));
                chase.AddTransition(new Framework.StateMachine.Transition(
                    new Framework.StateMachine.Conditions.TargetLostCondition(), idle));
                combat.AddTransition(new Framework.StateMachine.Transition(
                    new Framework.StateMachine.Conditions.TargetLostCondition(), chase));
                combat.AddTransition(new Framework.StateMachine.Transition(
                    new Framework.StateMachine.Conditions.WasHitCondition(), stagger));
                stagger.AddTransition(new Framework.StateMachine.Transition(
                    new Framework.StateMachine.Conditions.StaggerFinishedCondition(stagger), combat));

                return idle;
            }
        }
    }
}