using UnityEngine;

namespace Framework.AI.Memory
{
    public interface IMemoryContext
    {
        IAIMemory Memory { get; set; }
    }

    public interface IAIMemory
    {
        bool    HasTargetMemory    { get; }
        Vector3 LastKnownPosition { get; }
        void    Remember(Vector3 position, float time);
        void    Forget();
    }
}
