using System.Collections.Generic;
using UnityEngine;
using Framework.StateMachine;
using Framework.Core;

namespace Gameplay.AI
{
    // =========================================
    // AIAgentRegistry
    // Replaces the static Dictionary on AIController.
    // Registered as a service so its lifetime is
    // controlled by ServiceLocator — clears cleanly
    // on scene unload, no stale Transform references.
    //
    // Usage:
    //   ServiceLocator.Get<AIAgentRegistry>()?.Register(transform, context);
    //   ServiceLocator.Get<AIAgentRegistry>()?.TryGetContext(transform, out ctx);
    //
    // Place on your _GameSystems GameObject alongside
    // AIDirector, SquadSystem, AIGroupManager.
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
            if (transform == null || context == null)
                return;

            _agents[transform] = context;
        }

        public void Unregister(Transform transform)
        {
            if (transform == null)
                return;

            _agents.Remove(transform);
        }

        // =========================================
        // LOOKUP
        // =========================================
        public bool TryGetContext(Transform transform, out StateContext context)
        {
            return _agents.TryGetValue(transform, out context);
        }

        public StateContext GetContext(Transform transform)
        {
            _agents.TryGetValue(transform, out var context);
            return context;
        }
    }
}