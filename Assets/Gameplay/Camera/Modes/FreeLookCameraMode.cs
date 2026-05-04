using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class FreeLookCameraMode : ICameraMode
    {
        private Transform _target;
        private float _distance = 5f;
        private float _height = 2f;
        private float _rotationSpeed = 180f;
        private float _yaw;

        public FreeLookCameraMode(Transform target)
        {
            _target = target;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _yaw = cam.transform.eulerAngles.y;
        }

        public void Deactivate(UnityEngine.Camera cam)
        {
        }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_target == null)
                return;

            _yaw += Input.GetAxis("Mouse X") * _rotationSpeed * deltaTime;

            Vector3 offset = Quaternion.Euler(0f, _yaw, 0f) * new Vector3(0, 0, -_distance);
            Vector3 position = _target.position + offset + Vector3.up * _height;

            cam.transform.position = position;
            cam.transform.LookAt(_target.position + Vector3.up * 1.5f);
        }
    }
}