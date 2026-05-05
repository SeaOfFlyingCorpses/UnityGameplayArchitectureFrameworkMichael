using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // FPSCameraMode
    // Locks camera to head transform position
    // and follows head rotation (set by PlayerLook).
    //
    // Writes into CameraSnapshot — never touches
    // cam.transform directly. CameraModeController
    // applies the snapshot after all modes tick.
    // =========================================
    public class FPSCameraMode : ICameraMode
    {
        private readonly Transform _head;

        public FPSCameraMode(Transform head)
        {
            _head = head;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        public void Deactivate(UnityEngine.Camera cam)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_head == null)
                return;

            // Write into snapshot — CameraModeController applies it.
            // Writing directly to cam.transform gets overwritten by
            // snapshot.Apply() at the end of LateUpdate — always use snapshot.
            snapshot.Position = _head.position;
            snapshot.Rotation = _head.rotation;
        }
    }
}