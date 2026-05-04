using UnityEngine;
using Gameplay.Systems.Health;

namespace Gameplay.Combat
{
    public struct CombatTarget
    {
        public Transform Transform;
        public HealthComponent Health;
    }
}