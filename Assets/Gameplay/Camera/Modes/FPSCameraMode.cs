using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class FPSCameraMode : ICameraMode
    {
        private Transform _head;

        public FPSCameraMode(Transform head)
        {
            _head = head;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Deactivate(UnityEngine.Camera cam)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            cam.transform.position = _head.position;
            cam.transform.rotation = _head.rotation;
        }
    }
}