using Framework.AI.Systems;
using Framework.AI.Alert;
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
    // Lives in Gameplay — registers Gameplay systems.
    // Cannot live in Framework because it imports
    // Gameplay types. Framework.asmdef cannot see
    // Gameplay.asmdef.
    //
    // File location:
    //   Assets/Gameplay/AI/Systems/AISystemsBootstrap.cs
    // =========================================
    public static class AISystemsBootstrap
    {
        public static void RegisterDefaults(AISystemManager manager)
        {
            if (manager == null)
                return;

            manager.Register(new AILODSystem());
            manager.Register(new DirectorSystem());

            manager.Register(new SuppressionSystem());
            manager.Register(new MoralSystem());
            manager.Register(new AlertSystem());

            manager.Register(new ThreatSystem());
            manager.Register(new SquadAISystem());
            manager.Register(new FormationAISystem());
        }
    }
}
