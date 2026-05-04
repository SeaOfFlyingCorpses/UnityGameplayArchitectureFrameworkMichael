using UnityEngine;

namespace Gameplay.AI.Memory
{
    public class AIMemory
    {
        public Vector3 LastKnownPosition;
        public float LastSeenTime;

        public bool HasTargetMemory;

        // =========================================
        // MEMORY WRITE (called by states)
        // =========================================
        public void Remember(Vector3 position, float time)
        {
            LastKnownPosition = position;
            LastSeenTime = time;
            HasTargetMemory = true;
        }

        // =========================================
        // MEMORY CLEAR
        // =========================================
        public void Forget()
        {
            HasTargetMemory = false;
            LastKnownPosition = Vector3.zero;
            LastSeenTime = 0f;
        }

        // =========================================
        // OPTIONAL FUTURE HOOK (NOT USED YET)
        // =========================================
        public void TickDecay(float currentTime, float decayTime = 10f)
        {
            if (!HasTargetMemory)
                return;

            if (currentTime - LastSeenTime > decayTime)
            {
                Forget();
            }
        }
    }
}