using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class OrbitCameraMode : ICameraMode
    {
        private readonly Transform _focus;

        private float _yaw;
        private float _pitch;
        private float _distance;
        private float _autoRotateSpeed;

        private readonly Framework.Input.InputState _input;

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
            _yaw = _focus != null ? _focus.eulerAngles.y : 0f;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime,
            ref CameraSnapshot snapshot)
        {
            if (_focus == null) return;

            if (_input != null && _input.Look.sqrMagnitude > 0.01f)
            {
                _yaw   += _input.Look.x * 120f * deltaTime;
                _pitch -= _input.Look.y * 60f  * deltaTime;
                _pitch  = Mathf.Clamp(_pitch, -30f, 60f);
            }
            else
            {
                _yaw += _autoRotateSpeed * deltaTime;
            }

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3    position = _focus.position +
                                  rotation * new Vector3(0f, 0f, -_distance);

            snapshot.Position = position;
            snapshot.Rotation = Quaternion.LookRotation(
                _focus.position - position);
        }
    }
}