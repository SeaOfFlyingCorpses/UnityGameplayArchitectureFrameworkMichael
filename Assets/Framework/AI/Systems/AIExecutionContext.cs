namespace Framework.AI.Systems
{
    // =========================================
    // AIExecutionContext
    // Per-agent execution control data.
    // Lives on StateContext.Execution.
    //
    // LOD    — set each frame by AILODSystem
    //          read by AISystemManager to skip
    //          systems below the agent's current LOD
    //
    // NOTE: SystemMask lives on StateContext directly
    // as context.SystemMask — it's a flat field for
    // fast bitwise access by AISystemManager.
    // =========================================
    public class AIExecutionContext
    {
        public AILODLevel LOD = AILODLevel.High;
    }
}