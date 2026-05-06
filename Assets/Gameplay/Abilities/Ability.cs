using System;

namespace Gameplay.Abilities
{
    public class Ability
    {
        public string Id;
        public float  Cooldown;
        public float  LastUseTime;

        // Higher = preferred when multiple abilities ready
        public int Priority = 0;

        public Action<AbilityContext> Execute;

        public bool CanUse()
        {
            return UnityEngine.Time.time >= LastUseTime + Cooldown;
        }

        public void Use(AbilityContext context)
        {
            if (!CanUse()) return;

            Execute?.Invoke(context);
            LastUseTime = UnityEngine.Time.time;
        }
    }
}