using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // FreeLookCameraMode
    // Orbits around a target using mouse delta
    // from InputState — no legacy Input.GetAxis.
    //
    // Usage:
    //   var mode = new FreeLookCameraMode(target, inputState);
    //   cameraModeController.Request(new CameraRequest(mode, ...));
    // =========================================
    public class FreeLookCameraMode : ICameraMode
    {
        private readonly Transform  _target;
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
            _yaw = cam.transform.eulerAngles.y;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_target == null || _input == null)
                return;

            // Read mouse delta from InputState
            // InputState.Look is added below — see note
            _yaw += _input.Look.x * _rotationSpeed * deltaTime;

            Vector3 offset   = Quaternion.Euler(0f, _yaw, 0f) * new Vector3(0f, 0f, -_distance);
            Vector3 position = _target.position + offset + Vector3.up * _height;

            cam.transform.position = position;
            cam.transform.LookAt(_target.position + Vector3.up * 1.5f);
        }
    }
}