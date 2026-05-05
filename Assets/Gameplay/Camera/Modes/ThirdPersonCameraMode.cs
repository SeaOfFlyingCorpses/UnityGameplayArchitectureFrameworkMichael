using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // ThirdPersonCameraMode
    // Follows behind the target with smooth
    // damping and basic collision avoidance.
    //
    // Use case: action games, RPGs, adventure games
    // Examples: God of War, Zelda, Dark Souls
    //
    // Setup — CameraBootstrap:
    //   playerTarget = Player transform
    //   inputHandler = InputHandler on Player
    // =========================================
    public class ThirdPersonCameraMode : ICameraMode
    {
        private readonly Transform                  _target;
        private readonly Framework.Input.InputState _input;

        private float _yaw;
        private float _pitch;
        private float _currentDistance;

        private const float Distance      = 4f;
        private const float Height        = 1.5f;
        private const float PitchMin      = -20f;
        private const float PitchMax      = 60f;
        private const float RotationSpeed = 120f;
        private const float Damping       = 8f;
        private const float CollisionRadius = 0.2f;

        private Vector3 _smoothPosition;

        public ThirdPersonCameraMode(Transform target, Framework.Input.InputState input)
        {
            _target          = target;
            _input           = input;
            _currentDistance = Distance;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _yaw             = cam.transform.eulerAngles.y;
            _pitch           = 10f;
            _smoothPosition  = cam.transform.position;

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
            if (_target == null || _input == null)
                return;

            // Rotate with mouse
            _yaw   += _input.Look.x * RotationSpeed * deltaTime;
            _pitch -= _input.Look.y * RotationSpeed * deltaTime;
            _pitch  = Mathf.Clamp(_pitch, PitchMin, PitchMax);

            // Desired position behind target
            Quaternion rotation   = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 pivotPoint    = _target.position + Vector3.up * Height;
            Vector3 desiredPos    = pivotPoint + rotation * new Vector3(0f, 0f, -Distance);

            // Basic collision — shorten distance if something is in the way
            Vector3 dir = (desiredPos - pivotPoint).normalized;
            float   maxDist = Distance;

            if (Physics.SphereCast(pivotPoint, CollisionRadius, dir, out var hit, Distance))
                maxDist = Mathf.Clamp(hit.distance - CollisionRadius, 0.5f, Distance);

            _currentDistance = Mathf.Lerp(_currentDistance, maxDist, deltaTime * Damping);
            Vector3 finalPos  = pivotPoint + rotation * new Vector3(0f, 0f, -_currentDistance);

            // Smooth position
            _smoothPosition = Vector3.Lerp(_smoothPosition, finalPos, deltaTime * Damping);

            snapshot.Position = _smoothPosition;
            snapshot.Rotation = Quaternion.LookRotation(pivotPoint - _smoothPosition);
        }
    }
}
