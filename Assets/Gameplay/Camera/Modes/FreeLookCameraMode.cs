using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // FreeLookCameraMode
    // Orbits around a target using mouse delta
    // from InputState.
    // Writes into CameraSnapshot — never touches
    // cam.transform directly.
    // =========================================
    public class FreeLookCameraMode : ICameraMode
    {
        private readonly Transform                 _target;
        private readonly Framework.Input.InputState _input;

        private float _distance      = 5f;
        private float _height        = 2f;
        private float _rotationSpeed = 180f;
        private float _yaw;

        public FreeLookCameraMode(Transform target, Framework.Input.InputState input)
        {
            _target = target;
            _input  = input;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            // Start yaw from current camera angle
            _yaw = cam.transform.eulerAngles.y;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_target == null || _input == null)
                return;

            _yaw += _input.Look.x * _rotationSpeed * deltaTime;

            Vector3 offset   = Quaternion.Euler(0f, _yaw, 0f) * new Vector3(0f, 0f, -_distance);
            Vector3 position = _target.position + offset + Vector3.up * _height;

            // Write into snapshot — not cam.transform
            snapshot.Position = position;
            snapshot.Rotation = Quaternion.LookRotation(
                (_target.position + Vector3.up * 1.5f) - position
            );
        }
    }
}