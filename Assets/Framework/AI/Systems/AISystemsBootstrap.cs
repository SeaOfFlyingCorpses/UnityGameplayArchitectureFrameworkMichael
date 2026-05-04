using UnityEngine;
using Gameplay.AI.Suppression;
using Gameplay.AI.Moral;
using Gameplay.AI.Threat;
using Framework.AI.Alert;
using Gameplay.AI.Squad;

namespace Framework.AI.Systems
{
    public class AISystemsBootstrap : MonoBehaviour
    {
        private static bool _initialized = false;

        private void Awake()
        {
            if (_initialized)
                return;

            _initialized = true;

            // ✅ REGISTER ALL SYSTEMS ONCE
            AISystemManager.Register(new SuppressionSystem());
            AISystemManager.Register(new MoralSystem());
            AISystemManager.Register(new ThreatSystem());
            AISystemManager.Register(new AlertSystem());
            AISystemManager.Register(new SquadAISystem()); // 🔥 NEW
        }
    }
}