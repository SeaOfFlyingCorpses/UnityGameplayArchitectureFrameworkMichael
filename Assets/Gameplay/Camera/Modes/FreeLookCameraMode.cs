using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class FreeLookCameraMode : ICameraMode
    {
        private readonly Transform                  _target;
        private readonly Framework.Input.InputState _input;

        private readonly float _distance;
        private readonly float _height;
        private const    float RotationSpeed = 180f;
        private float _yaw;

        public FreeLookCameraMode(
            Transform                  target,
            Framework.Input.InputState input,
            float                      distance = 5f,
            float                      height   = 3f)
        {
            _target   = target;
            _input    = input;
            _distance = distance;
            _height   = height;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _yaw = _target != null ? _target.eulerAngles.y : 0f;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime,
            ref CameraSnapshot snapshot)
        {
            if (_target == null || _input == null) return;

            _yaw += _input.Look.x * RotationSpeed * deltaTime;

            Vector3 offset   = Quaternion.Euler(0f, _yaw, 0f) *
                               new Vector3(0f, 0f, -_distance);
            Vector3 position = _target.position + offset +
                               Vector3.up * _height;

            snapshot.Position = position;
            snapshot.Rotation = Quaternion.LookRotation(
                _target.position - position);
        }
    }
}