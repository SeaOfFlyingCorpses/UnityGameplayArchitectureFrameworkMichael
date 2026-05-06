using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class ThirdPersonCameraMode : ICameraMode
    {
        private readonly Transform                  _target;
        private readonly Framework.Input.InputState _input;

        private float _yaw;
        private float _pitch;
        private bool  _initialized;

        private const float Distance       = 4f;
        private const float Height         = 1.5f;
        private const float PitchMin       = -20f;
        private const float PitchMax       = 60f;
        private const float RotationSpeed  = 120f;
        private const float Damping        = 8f;

        private Vector3 _smoothPosition;

        public ThirdPersonCameraMode(Transform target, Framework.Input.InputState input)
        {
            _target          = target;
            _input           = input;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _yaw         = _target != null ? _target.eulerAngles.y : 0f;
            _pitch       = 10f;
            _initialized = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        public void Deactivate(UnityEngine.Camera cam)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }

        public void Tick(UnityEngine.Camera cam, float deltaTime,
            ref CameraSnapshot snapshot)
        {
            if (_target == null || _input == null) return;

            _yaw   += _input.Look.x * RotationSpeed * deltaTime;
            _pitch -= _input.Look.y * RotationSpeed * deltaTime;
            _pitch  = Mathf.Clamp(_pitch, PitchMin, PitchMax);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 pivotPoint  = _target.position + Vector3.up * Height;
            Vector3 desiredPos  = pivotPoint + rotation *
                                  new Vector3(0f, 0f, -Distance);

            if (!_initialized)
            {
                _smoothPosition = desiredPos;
                _initialized    = true;
            }
            else
            {
                _smoothPosition = Vector3.Lerp(
                    _smoothPosition, desiredPos, deltaTime * Damping);
            }

            snapshot.Position = _smoothPosition;
            snapshot.Rotation = Quaternion.LookRotation(
                pivotPoint - _smoothPosition);
        }
    }
}