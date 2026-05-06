using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class IsometricCameraMode : ICameraMode
    {
        private readonly Transform _target;
        private readonly float     _height;
        private readonly float     _distance;
        private readonly float     _smoothSpeed;
        private readonly float     _yawAngle;

        private Vector3 _smoothPosition;
        private bool    _initialized;

        public IsometricCameraMode(
            Transform target,
            float height      = 12f,
            float distance    = 12f,
            float yawAngle    = 45f,
            float smoothSpeed = 6f)
        {
            _target      = target;
            _height      = height;
            _distance    = distance;
            _yawAngle    = yawAngle;
            _smoothSpeed = smoothSpeed;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _initialized = false;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime,
            ref CameraSnapshot snapshot)
        {
            if (_target == null) return;

            float   rad    = _yawAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                -Mathf.Sin(rad) * _distance,
                _height,
                Mathf.Cos(rad) * _distance);

            Vector3 desired = _target.position + offset;

            if (!_initialized)
            {
                _smoothPosition = desired;
                _initialized    = true;
            }
            else
            {
                _smoothPosition = Vector3.Lerp(
                    _smoothPosition, desired, deltaTime * _smoothSpeed);
            }

            snapshot.Position = _smoothPosition;
            snapshot.Rotation = Quaternion.LookRotation(
                _target.position - _smoothPosition);
        }
    }
}