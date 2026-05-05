using Framework.AI.Memory;

namespace Gameplay.AI.Memory
{
    public class MemoryContext : IMemoryContext
    {
        public IAIMemory Memory { get; set; } = new AIMemory();
    }
}