using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // FixedCameraMode
    // Camera stays at a fixed world position
    // and tracks a moving target.
    // Blend between fixed cameras using
    // CameraModeController.Request() with
    // a blend time to get smooth cuts.
    //
    // Use case: survival horror, puzzle games,
    //           scripted moments, security cameras
    // Examples: Resident Evil 1-3, Silent Hill
    // =========================================
    public class FixedCameraMode : ICameraMode
    {
        private readonly Vector3    _position;
        private readonly Transform  _lookAt;     // null = use fixed rotation
        private readonly Quaternion _fixedRotation;

        // Fixed position, tracking a target
        public FixedCameraMode(Vector3 position, Transform lookAt)
        {
            _position      = position;
            _lookAt        = lookAt;
            _fixedRotation = Quaternion.identity;
        }

        // Fixed position, fixed rotation — pure static shot
        public FixedCameraMode(Vector3 position, Quaternion rotation)
        {
            _position      = position;
            _lookAt        = null;
            _fixedRotation = rotation;
        }

        public void Activate(UnityEngine.Camera cam)   { }
        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            snapshot.Position = _position;

            if (_lookAt != null)
            {
                Vector3 dir = (_lookAt.position - _position);
                if (dir.sqrMagnitude > 0.001f)
                    snapshot.Rotation = Quaternion.LookRotation(dir.normalized);
            }
            else
            {
                snapshot.Rotation = _fixedRotation;
            }
        }
    }
}
