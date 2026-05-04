using System.Collections.Generic;
using Framework.StateMachine;
using UnityEngine;
using Framework.AI.Systems;

namespace Gameplay.AI.Group
{
    public class AIGroupManager : MonoBehaviour
    {
        public static AIGroupManager Instance;

        private List<StateContext> _agents = new();

        public Vector3 SharedLastKnownPosition;
        public float SharedAlertLevel;

        private AIGroupSystem _system;

        private void Awake()
        {
            Instance = this;

            // register system into AI pipeline
            _system = new AIGroupSystem(this);
            AISystemManager.Register(_system);
        }

        private void OnDestroy()
        {
            if (_system != null)
                AISystemManager.Unregister(_system);
        }

        // =========================================
        // AGENT REGISTRATION (UNCHANGED)
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

            if (_agents.Contains(context))
                _agents.Remove(context);
        }

        // =========================================
        // GLOBAL SIGNAL
        // =========================================
        public void BroadcastAlert(Vector3 position, float alertStrength)
        {
            SharedLastKnownPosition = position;
            SharedAlertLevel = Mathf.Max(SharedAlertLevel, alertStrength);
        }

        // =========================================
        // ACCESS FOR SYSTEM
        // =========================================
        public List<StateContext> GetAgents()
        {
            return _agents;
        }
    }
}