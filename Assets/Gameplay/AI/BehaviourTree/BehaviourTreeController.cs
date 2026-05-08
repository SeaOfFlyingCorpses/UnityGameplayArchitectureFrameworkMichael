using System.Collections.Generic;
using UnityEngine;
using Framework.Commands;
using Framework.Input;
using Framework.AI.Systems;
using Framework.Core;
using Framework.AI.Faction;
using Framework.StateMachine;
using Gameplay.AI.Perception;
using Gameplay.AI.Group;
using Gameplay.AI.Squad;
using Gameplay.AI.Director;
using Gameplay.AI.Memory;
using Gameplay.AI.Systems;
using Gameplay.Systems.Health;
using Gameplay.Abilities;
using Gameplay.Combat;
using Gameplay.Systems.Movement;
using UnityEngine.AI;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // BehaviourTreeController
    // Like AIController but drives a behaviour
    // tree instead of a state machine.
    //
    // Subclass and override BuildTree() to
    // define the agent's behaviour.
    //
    // Setup:
    //   1. Create a subclass:
    //      public class GuardController
    //          : BehaviourTreeController
    //      {
    //          protected override IBehaviourTree
    //              BuildTree() => new ...
    //      }
    //   2. Add to enemy GameObject
    //   3. Works with all existing systems
    //      (perception, health, squad, etc.)
    // =========================================
    public abstract class BehaviourTreeController : MonoBehaviour
    {
        private BehaviourTree   _tree;
        private StateContext    _context;
        private AISystemManager _aiSystems;
        private bool            _initialized;

        [Header("References")]
        public PerceptionSystem perception;
        public HealthComponent  healthComponent;
        public Rigidbody        rb;

        [Header("Player Detection")]
        public string playerTag = "Player";

        private Transform _playerTransform;

        [Header("AI Settings")]
        public Team team;

        private void Awake()  => Initialize();
        private void Start()  => RegisterSquad();

        private void OnEnable()
        {
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
            if (!_initialized) return;

            _context.Movement?.Tick(_context);
            perception?.Tick();
            _aiSystems?.UpdateAll(_context);
            _tree?.Tick(_context);
            _context.WasHit = false;
            _context.Commands?.ExecuteAll();
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<AIAgentRegistry>()?.Unregister(transform);
            ServiceLocator.Get<AIGroupManager>()?.Unregister(_context);
        }

        private void Initialize()
        {
            if (_initialized) return;

            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
                _playerTransform = playerObj.transform;

            CreateContext();
            BindSystems();

            // Build the behaviour tree
            var root = BuildTree();
            _tree = new BehaviourTree(root);

            _initialized = true;
        }

        private void RegisterSquad()
        {
            ServiceLocator.Get<SquadSystem>()?.Register(_context);
        }

        // =========================================
        // OVERRIDE THIS — define agent behaviour
        // =========================================
        protected abstract Framework.AI.BehaviourTree.IBehaviourNode
            BuildTree();

        private void CreateContext()
        {
            _context = new Framework.StateMachine.StateContext
            {
                Input    = new InputState(),
                Commands = new CommandQueue(),

                HealthData = healthComponent?.GetHealth(),
                HealthComp = healthComponent,
                Self       = transform,

                PerceptionContext = new PerceptionContext
                {
                    State          = new PerceptionState(),
                    Target         = _playerTransform,
                    VisibleTargets = new List<Transform>()
                },

                MemoryContext = new MemoryContext(),
                AlertContext  = new Framework.AI.Alert.AlertContext(),
                SquadContext  = new SquadContext(),
                Team          = team,
                Abilities     = new AbilitySystem(),
                StatusEffects = new Gameplay.StatusEffects.StatusEffectSystem()
            };
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

            if (perception != null)
            {
                perception.context = _context;
                perception.target  = _playerTransform;
            }

            var navAgent = GetComponent<NavMeshAgent>();
            var rb2d     = GetComponent<Rigidbody2D>();

            if (navAgent != null)
                _context.Movement = new NavMeshMovementStrategy(navAgent);
            else if (rb2d != null && rb2d.gravityScale == 0f)
                _context.Movement = new TopDown2DMovementStrategy(rb2d);
            else if (rb2d != null)
                _context.Movement = new PlatformerMovementStrategy(rb2d);
            else
                _context.Movement = new TransformMovementStrategy();

            if (healthComponent != null)
            {
                healthComponent.OnHit += OnHit;
                healthComponent.OnDeath += () => enabled = false;
            }
        }

        private void OnHit(Vector3 hitPoint)
        {
            _context.WasHit       = true;
            _context.HitDirection =
                (transform.position - hitPoint).normalized;
        }
    }
}
