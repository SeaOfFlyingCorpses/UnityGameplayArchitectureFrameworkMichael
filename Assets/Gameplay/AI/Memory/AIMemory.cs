using UnityEngine;
using Framework.AI.Memory;

namespace Gameplay.AI.Memory
{
    public class AIMemory : IAIMemory
    {
        public Vector3 LastKnownPosition { get; private set; }
        public float   LastSeenTime      { get; private set; }
        public bool    HasTargetMemory   { get; private set; }

        public void Remember(Vector3 position, float time)
        {
            LastKnownPosition = position;
            LastSeenTime      = time;
            HasTargetMemory   = true;
        }

        public void Forget()
        {
            HasTargetMemory   = false;
            LastKnownPosition = Vector3.zero;
            LastSeenTime      = 0f;
        }

        // Optional decay — not part of interface, Gameplay-only
        public void TickDecay(float currentTime, float decayTime = 10f)
        {
            if (!HasTargetMemory) return;
            if (currentTime - LastSeenTime > decayTime)
                Forget();
        }
    }
}