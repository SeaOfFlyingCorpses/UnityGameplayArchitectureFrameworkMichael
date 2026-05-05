using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // TopDownCameraMode
    // Fixed overhead angle, follows a target.
    // Optionally allows zoom with scroll wheel.
    //
    // Use case: RTS, MOBA, twin-stick shooters
    // Examples: Diablo, Hades, StarCraft
    // =========================================
    public class TopDownCameraMode : ICameraMode
    {
        private readonly Transform _target;

        private float _height;
        private float _tilt;
        private float _smoothSpeed;

        private Vector3 _smoothPosition;

        public TopDownCameraMode(
            Transform target,
            float height      = 15f,
            float tilt        = 70f,
            float smoothSpeed = 5f)
        {
            _target      = target;
            _height      = height;
            _tilt        = tilt;
            _smoothSpeed = smoothSpeed;
        }

        public void Activate(UnityEngine.Camera cam) { }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_target == null)
                return;

            // Position directly above and slightly behind target
            Vector3 offset  = Quaternion.Euler(_tilt, 0f, 0f) * new Vector3(0f, 0f, -_height);
            Vector3 desired = _target.position + offset;

            _smoothPosition = Vector3.Lerp(_smoothPosition, desired, deltaTime * _smoothSpeed);

            snapshot.Position = _smoothPosition;
            snapshot.Rotation = Quaternion.Euler(_tilt, 0f, 0f);
        }
    }
}
