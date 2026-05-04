using UnityEngine;
using Gameplay.Systems.Health;

namespace Gameplay.Combat
{
    public class Targetable : MonoBehaviour
    {
        public HealthComponent Health;

        private void Awake()
        {
            if (Health == null)
                Health = GetComponent<HealthComponent>();
        }
    }
}