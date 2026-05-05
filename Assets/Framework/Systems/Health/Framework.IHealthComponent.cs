using System;
using UnityEngine;

namespace Framework.Systems.Health
{
    public interface IHealthComponent
    {
        IHealth              GetHealth();
        void                 Damage(int amount, Vector3 hitPoint);
        event Action<Vector3> OnHit;
        event Action          OnDeath;
    }
}
