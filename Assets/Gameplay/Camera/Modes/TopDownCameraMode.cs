using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class TopDownCameraMode : ICameraMode
    {
        private readonly Transform _target;
        private readonly float     _height;
        private readonly float     _smoothSpeed;

        private Vector3 _smoothPosition;
        private bool    _initialized;

        public TopDownCameraMode(
            Transform target,
            float height      = 15f,
            float smoothSpeed = 5f)
        {
            _target      = target;
            _height      = height;
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

            Vector3 desired = _target.position + Vector3.up * _height;

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