using System.Collections.Generic;
using Framework.StateMachine;
using Framework.Core;
using UnityEngine;

namespace Gameplay.AI.Group
{
    public class AIGroupManager : MonoBehaviour
    {
        // =========================================
        // No more "public static Instance"
        // Retrieve via: ServiceLocator.Get<AIGroupManager>()
        // =========================================

        private readonly List<StateContext> _agents = new();

        public Vector3 SharedLastKnownPosition;
        public float   SharedAlertLevel;

        private void Awake()
        {
            ServiceLocator.Register<AIGroupManager>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<AIGroupManager>();
        }

        // =========================================
        // AGENT REGISTRATION — unchanged
        // =========================================
        public void Register(StateContext context)
        {
            if (context == null)
                return;

            if (!_agents.Contains(context))
                _agents.Add(context);
        }

        public void Unregister(StateContext context)
        {
            if (context == null)
                return;

            _agents.Remove(context);
        }

        // =========================================
        // GLOBAL SIGNAL — unchanged
        // =========================================
        public void BroadcastAlert(Vector3 position, float alertStrength)
        {
            SharedLastKnownPosition = position;
            SharedAlertLevel = Mathf.Max(SharedAlertLevel, alertStrength);
        }

        // =========================================
        // ACCESS FOR SYSTEM — unchanged
        // =========================================
        public List<StateContext> GetAgents()
        {
            return _agents;
        }
    }
}