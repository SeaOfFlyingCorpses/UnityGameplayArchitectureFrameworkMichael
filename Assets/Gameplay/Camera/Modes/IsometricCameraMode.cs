using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // IsometricCameraMode
    // Fixed 45-degree isometric angle.
    // Smoothly follows a target.
    //
    // Use case: classic isometric games
    // Examples: Diablo 2, Path of Exile,
    //           Baldur's Gate, Age of Empires
    // =========================================
    public class IsometricCameraMode : ICameraMode
    {
        private readonly Transform _target;

        private readonly float   _height;
        private readonly float   _distance;
        private readonly float   _smoothSpeed;
        private readonly float   _angle;      // horizontal rotation — classic is 45

        private Vector3 _smoothPosition;

        public IsometricCameraMode(
            Transform target,
            float height      = 12f,
            float distance    = 12f,
            float angle       = 45f,
            float smoothSpeed = 6f)
        {
            _target      = target;
            _height      = height;
            _distance    = distance;
            _angle       = angle;
            _smoothSpeed = smoothSpeed;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _smoothPosition = cam.transform.position;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_target == null)
                return;

            // Fixed offset at isometric angle
            Vector3 offset = Quaternion.Euler(0f, _angle, 0f)
                           * new Vector3(0f, _height, -_distance);

            Vector3 desired = _target.position + offset;

            _smoothPosition = Vector3.Lerp(
                _smoothPosition, desired, deltaTime * _smoothSpeed);

            snapshot.Position = _smoothPosition;
            snapshot.Rotation = Quaternion.LookRotation(
                _target.position - _smoothPosition);
        }
    }
}
