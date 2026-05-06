using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class OverShoulderCameraMode : ICameraMode
    {
        private readonly Transform                  _target;
        private readonly Framework.Input.InputState _input;

        private float _yaw;
        private float _pitch;

        private const float RotationSpeed  = 120f;
        private const float PitchMin       = -30f;
        private const float PitchMax       =  50f;
        private const float ShoulderOffset =  0.6f;
        private const float HeightOffset   =  1.4f;
        private const float DepthOffset    = -2.0f;

        public OverShoulderCameraMode(Transform target,
            Framework.Input.InputState input)
        {
            _target = target;
            _input  = input;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _yaw   = _target != null ? _target.eulerAngles.y : 0f;
            _pitch = 0f;

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

            _target.rotation = Quaternion.Euler(0f, _yaw, 0f);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 localOffset = new Vector3(
                ShoulderOffset, HeightOffset, DepthOffset);

            snapshot.Position = _target.position +
                                 rotation * localOffset;
            snapshot.Rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}