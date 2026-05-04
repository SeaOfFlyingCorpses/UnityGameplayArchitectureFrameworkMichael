using UnityEngine;
using Gameplay.Systems.Health;

namespace Gameplay.Abilities
{
    public class AbilityContext
    {
        public Transform Self;
        public Transform Target;

        public HealthComponent SourceHealth;
        public HealthComponent TargetHealth;
    }
}