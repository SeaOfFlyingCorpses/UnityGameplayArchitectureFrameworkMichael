namespace Gameplay.Camera
{
    public interface ICameraMode
    {
        void Activate(UnityEngine.Camera cam);
        void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot);
        void Deactivate(UnityEngine.Camera cam);
    }
}