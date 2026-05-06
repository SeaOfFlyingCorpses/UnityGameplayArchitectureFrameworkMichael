using UnityEngine;

namespace Gameplay.Camera
{
    // =========================================
    // CameraSnapshot2D
    // Holds the desired camera state for 2D.
    // All 2D camera modes write into this.
    // CameraModeController2D applies it.
    //
    // Position — XY world position
    // OrthoSize — orthographic size (zoom)
    // =========================================
    public struct CameraSnapshot2D
    {
        public Vector3 Position;
        public float   OrthoSize;
        public float   Rotation; // Z rotation only for 2D

        public static CameraSnapshot2D Capture(UnityEngine.Camera cam)
        {
            return new CameraSnapshot2D
            {
                Position  = cam.transform.position,
                OrthoSize = cam.orthographicSize,
                Rotation  = cam.transform.eulerAngles.z
            };
        }

        public void Apply(UnityEngine.Camera cam)
        {
            cam.transform.position    = Position;
            cam.orthographicSize      = OrthoSize;
            cam.transform.eulerAngles =
                new Vector3(0f, 0f, Rotation);
        }

        public static CameraSnapshot2D Lerp(
            CameraSnapshot2D a,
            CameraSnapshot2D b,
            float            t)
        {
            return new CameraSnapshot2D
            {
                Position  = Vector3.Lerp(a.Position, b.Position, t),
                OrthoSize = Mathf.Lerp(a.OrthoSize, b.OrthoSize, t),
                Rotation  = Mathf.LerpAngle(a.Rotation, b.Rotation, t)
            };
        }
    }
}