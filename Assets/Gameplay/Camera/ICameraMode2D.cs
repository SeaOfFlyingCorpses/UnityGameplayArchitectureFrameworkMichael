using UnityEngine;

namespace Gameplay.Camera
{
    // =========================================
    // ICameraMode2D
    // Contract for all 2D camera modes.
    // Same pattern as ICameraMode for 3D.
    // All modes write into CameraSnapshot2D.
    // =========================================
    public interface ICameraMode2D
    {
        void Activate(UnityEngine.Camera cam);
        void Deactivate(UnityEngine.Camera cam);
        void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot2D snapshot);
    }
}