using Framework.StateMachine;

namespace Framework.AI.Systems
{
    public interface IAISystem
    {
        AISystemCategory Category { get; }

        void Update(StateContext context);

        // =========================================
        // CONDITIONAL EXECUTION (OPTIONAL OVERRIDE)
        // =========================================
        bool ShouldRun(StateContext context) => true;

        // =========================================
        // LOD FILTER (optional override)
        // =========================================
        AILODLevel MinLOD => AILODLevel.Low;

        // =========================================
        // SYSTEM MASK (for control layer)
        // =========================================
        ulong Mask => ulong.MaxValue;
    }
}