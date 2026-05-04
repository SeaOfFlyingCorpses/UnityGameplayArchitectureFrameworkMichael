using UnityEngine;

namespace Gameplay.AI.Memory
{
    public class MemoryContext
    {
        // =========================================
        // CORE MEMORY OBJECT
        // =========================================
        public AIMemory Memory = new AIMemory();

        // =========================================
        // FUTURE EXTENSIONS (NOT USED YET)
        // =========================================
        // public float MemoryStrength;
        // public float ForgetRate;
        // public Transform LastKnownTarget;
    }
}