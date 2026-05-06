using Framework.AI.Systems;
using Framework.AI.Alert;
using UnityEngine;
using Gameplay.AI.Suppression;
using Gameplay.AI.Moral;
using Gameplay.AI.Threat;
using Gameplay.AI.Systems;
using Gameplay.AI.Squad;
using Gameplay.AI.LOD;

namespace Gameplay.AI.Systems
{
    // =========================================
    // AISystemsBootstrap
    // Registers all default AI systems.
    // Lives in Gameplay — imports Gameplay types.
    //
    // 3D systems registered by default.
    // 2D systems opt-in via Register2D().
    //
    // Usage — 3D agent:
    //   AISystemsBootstrap.RegisterDefaults(manager);
    //
    // Usage — 2D platformer agent:
    //   AISystemsBootstrap.RegisterDefaults(manager);
    //   AISystemsBootstrap.Register2D(manager,
    //       LayerMask.GetMask("Ground"));
    // =========================================
    public static class AISystemsBootstrap
    {
        public static void RegisterDefaults(AISystemManager manager)
        {
            if (manager == null) return;

            manager.Register(new AILODSystem());
            manager.Register(new DirectorSystem());

            manager.Register(new SuppressionSystem());
            manager.Register(new MoralSystem());
            manager.Register(new AlertSystem());

            manager.Register(new ThreatSystem());
            manager.Register(new SquadAISystem());
            manager.Register(new FormationAISystem());
        }

        // =========================================
        // Register2D
        // Call after RegisterDefaults for any
        // agent using PlatformerMovementStrategy.
        // Adds ledge detection so agents stop
        // before walking off platforms.
        // =========================================
        public static void Register2D(
            AISystemManager manager,
            LayerMask       groundLayer)
        {
            if (manager == null) return;

            manager.Register(new LedgeDetectionSystem(groundLayer));
        }
    }
}