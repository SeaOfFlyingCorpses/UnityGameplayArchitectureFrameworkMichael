using UnityEngine;
using Gameplay.Systems.Health;

namespace Gameplay.Combat
{
    // =========================================
    // Targetable
    // Must be on any GameObject that can be
    // attacked. CombatState resolves HealthComponent
    // through this component.
    //
    // If HealthComponent is missing, attacks
    // silently do nothing — the warning below
    // makes that immediately obvious.
    // =========================================
    public class Targetable : MonoBehaviour
    {
        public HealthComponent Health;

        private void Awake()
        {
            if (Health == null)
                Health = GetComponent<HealthComponent>();

            if (Health == null)
                Debug.LogWarning(
                    $"[Targetable] {gameObject.name} has no HealthComponent — " +
                    $"attacks will deal no damage. Add a HealthComponent.");
        }
    }
}