using Framework.AI.Systems;
using Framework.StateMachine;
using Framework.Core;
using Gameplay.AI.Director;

namespace Gameplay.AI.Systems
{
    // =========================================
    // DirectorSystem
    // Reads global director intensity and writes
    // it into each agent's StateContext.DirectorIntensity.
    //
    // This keeps framework states (CombatState etc.)
    // free of any ServiceLocator or AIDirector imports —
    // they just read context.DirectorIntensity.
    //
    // If no AIDirector is registered, intensity stays 0.
    // Register in AISystemsBootstrap.RegisterDefaults().
    // =========================================
    public class DirectorSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Utility;

        public void Update(StateContext context)
        {
            var director = ServiceLocator.Get<AIDirector>();

            context.DirectorIntensity = director != null
                ? director.State.Intensity
                : 0f;
        }
    }
}