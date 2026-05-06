using UnityEngine;

namespace Gameplay.Camera
{
    public struct CameraSnapshot
    {
        public Vector3    Position;
        public Quaternion Rotation;
        public float      FOV;

        public static CameraSnapshot Capture(UnityEngine.Camera cam)
        {
            return new CameraSnapshot
            {
                Position = cam.transform.position,
                Rotation = cam.transform.rotation,
                FOV      = cam.fieldOfView
            };
        }

        public static CameraSnapshot Lerp(CameraSnapshot a, CameraSnapshot b, float t)
        {
            return new CameraSnapshot
            {
                Position = Vector3.Lerp(a.Position, b.Position, t),
                Rotation = Quaternion.Lerp(a.Rotation, b.Rotation, t),
                FOV      = Mathf.Lerp(a.FOV, b.FOV, t)
            };
        }

        public void Apply(UnityEngine.Camera cam)
        {
            cam.transform.position = Position;
            cam.transform.rotation = Rotation;
            cam.fieldOfView        = FOV;
        }
    }
}