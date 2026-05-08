using Unity.Profiling;

namespace Framework.Core
{
    // =========================================
    // FrameworkProfiler
    // Central registry of all profiler markers.
    // Zero allocation — markers are structs.
    // Zero cost in release builds —
    // Unity strips profiler code automatically.
    //
    // Usage:
    //   using (FrameworkProfiler.AISystemUpdate
    //       .Auto())
    //   {
    //       _aiSystems.UpdateAll(context);
    //   }
    // =========================================
    public static class FrameworkProfiler
    {
        // =========================================
        // AI
        // =========================================
        public static readonly ProfilerMarker AISystemUpdate =
            new ProfilerMarker("Framework.AISystemManager.Update");

        public static readonly ProfilerMarker AIStateUpdate =
            new ProfilerMarker("Framework.StateMachine.Update");

        public static readonly ProfilerMarker AIPerceptionTick =
            new ProfilerMarker("Framework.Perception.Tick");

        // =========================================
        // COMMANDS
        // =========================================
        public static readonly ProfilerMarker CommandExecute =
            new ProfilerMarker("Framework.CommandQueue.ExecuteAll");

        // =========================================
        // POOL
        // =========================================
        public static readonly ProfilerMarker PoolGet =
            new ProfilerMarker("Framework.Pool.Get");

        public static readonly ProfilerMarker PoolReturn =
            new ProfilerMarker("Framework.Pool.Return");

        // =========================================
        // EVENTS
        // =========================================
        public static readonly ProfilerMarker EventPublish =
            new ProfilerMarker("Framework.EventBus.Publish");

        // =========================================
        // CAMERA
        // =========================================
        public static readonly ProfilerMarker CameraTick =
            new ProfilerMarker("Framework.CameraMode.Tick");
    }
}
