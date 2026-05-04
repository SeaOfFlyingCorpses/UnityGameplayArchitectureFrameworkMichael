namespace Gameplay.Camera
{
    public enum CameraPriority
    {
        Low,
        Normal,
        High,
        Override
    }

    public struct CameraRequest
    {
        public ICameraMode Mode;
        public CameraPriority Priority;
        public float BlendTime; 

        public CameraRequest(ICameraMode mode, CameraPriority priority, float blendTime)
        {
            Mode = mode;
            Priority = priority;
            BlendTime = blendTime;
        }
    }
}