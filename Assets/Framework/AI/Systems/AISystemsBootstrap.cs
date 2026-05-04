using Framework.AI.Systems;
using Gameplay.AI.Suppression;
using Gameplay.AI.Moral;
using Gameplay.AI.Threat;
using Gameplay.AI.Systems;
using Framework.AI.Alert;
using Gameplay.AI.Squad;

namespace Framework.AI.Systems
{
    // =========================================
    // AISystemsBootstrap
    // Populates an AISystemManager instance with
    // the standard set of AI systems.
    //
    // Usage — called from AIController.BindSystems():
    //   AISystemsBootstrap.RegisterDefaults(_aiSystems);
    //
    // To build a custom agent, skip this and register
    // only what you need manually.
    // =========================================
    public static class AISystemsBootstrap
    {
        public static void RegisterDefaults(AISystemManager manager)
        {
            if (manager == null)
                return;

            // Utility — runs first so intensity/squad data
            // is ready before Emotion systems read it
            manager.Register(new DirectorSystem());

            // Emotion
            manager.Register(new SuppressionSystem());
            manager.Register(new MoralSystem());
            manager.Register(new AlertSystem());

            // Combat / Threat
            manager.Register(new ThreatSystem());

            // Squad
            manager.Register(new SquadAISystem());
        }
    }
}