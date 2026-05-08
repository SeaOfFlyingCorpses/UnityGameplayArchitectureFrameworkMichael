using System.Collections.Generic;
using UnityEngine;
using Framework.StateMachine;
using Framework.Core;

namespace Gameplay.AI
{
    // =========================================
    // AIAgentData
    // Snapshot of one registered agent.
    // Used by AIDebugOverlay and editor tools.
    // =========================================
    public class AIAgentData
    {
        public Transform     Transform;
        public StateContext  Context;
    }

    // =========================================
    // AIAgentRegistry
    // Central registry of all active AI agents.
    // Place on _GameSystems.
    // =========================================
    public class AIAgentRegistry : MonoBehaviour
    {
        private readonly Dictionary<Transform, StateContext> _agents = new();

        private void Awake()
        {
            ServiceLocator.Register<AIAgentRegistry>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<AIAgentRegistry>();
            _agents.Clear();
        }

        // =========================================
        // REGISTER / UNREGISTER
        // =========================================
        public void Register(Transform transform, StateContext context)
        {
            if (transform == null || context == null) return;
            _agents[transform] = context;
        }

        public void Unregister(Transform transform)
        {
            if (transform == null) return;
            _agents.Remove(transform);
        }

        // =========================================
        // LOOKUP
        // =========================================
        public bool TryGetContext(Transform transform,
                                  out StateContext context)
            => _agents.TryGetValue(transform, out context);

        public StateContext GetContext(Transform transform)
        {
            _agents.TryGetValue(transform, out var context);
            return context;
        }

        // =========================================
        // GET ALL — used by debug overlay and tools
        // =========================================
        public IReadOnlyList<AIAgentData> GetAll()
        {
            var list = new List<AIAgentData>(_agents.Count);

            foreach (var kvp in _agents)
            {
                if (kvp.Key == null) continue;
                list.Add(new AIAgentData
                {
                    Transform = kvp.Key,
                    Context   = kvp.Value
                });
            }

            return list;
        }

        public int Count => _agents.Count;
    }
}