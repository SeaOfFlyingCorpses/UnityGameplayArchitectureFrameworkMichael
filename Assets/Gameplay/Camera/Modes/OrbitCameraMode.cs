using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // OrbitCameraMode
    // Continuously orbits a fixed point.
    // No input needed — runs automatically.
    // Can also be driven by mouse drag input.
    //
    // Use case: main menus, character select,
    //           item inspection, victory screens
    // Examples: most RPG character screens
    // =========================================
    public class OrbitCameraMode : ICameraMode
    {
        private readonly Transform _focus;

        private float _yaw;
        private float _pitch;
        private float _distance;
        private float _autoRotateSpeed;

        private readonly Framework.Input.InputState _input; // optional — null = auto rotate

        public OrbitCameraMode(
            Transform focus,
            float distance        = 4f,
            float pitch           = 15f,
            float autoRotateSpeed = 20f,
            Framework.Input.InputState input = null)
        {
            _focus           = focus;
            _distance        = distance;
            _pitch           = pitch;
            _autoRotateSpeed = autoRotateSpeed;
            _input           = input;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _yaw = cam.transform.eulerAngles.y;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_focus == null)
                return;

            if (_input != null && _input.Look.sqrMagnitude > 0.01f)
            {
                // Manual control when input is present
                _yaw   += _input.Look.x * 120f * deltaTime;
                _pitch -= _input.Look.y * 60f  * deltaTime;
                _pitch  = Mathf.Clamp(_pitch, -30f, 60f);
            }
            else
            {
                // Auto rotate when no input
                _yaw += _autoRotateSpeed * deltaTime;
            }

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3    position = _focus.position + rotation * new Vector3(0f, 0f, -_distance);

            snapshot.Position = position;
            snapshot.Rotation = Quaternion.LookRotation(_focus.position - position);
        }
    }
}
