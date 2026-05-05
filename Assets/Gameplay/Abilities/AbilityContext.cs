using UnityEngine;
using Framework.Systems.Health;

namespace Gameplay.Abilities
{
    public class AbilityContext
    {
        public Transform     Self;
        public Transform     Target;
        public IHealthComponent SourceHealth;
        public IHealthComponent TargetHealth;
    }
}