using UnityEngine;

namespace Gameplay.Camera.Modes
{
    // =========================================
    // CinematicCameraMode
    // Smoothly travels between two transforms.
    // Loops back and forth (ping-pong).
    // Writes into CameraSnapshot — never touches
    // cam.transform directly.
    // =========================================
    public class CinematicCameraMode : ICameraMode
    {
        private readonly Transform _pointA;
        private readonly Transform _pointB;

        private float _t;
        private float _speed   = 0.2f;
        private bool  _forward = true;

        public CinematicCameraMode(Transform a, Transform b, float speed = 0.2f)
        {
            _pointA = a;
            _pointB = b;
            _speed  = speed;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _t       = 0f;
            _forward = true;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_pointA == null || _pointB == null)
                return;

            _t += (_forward ? 1f : -1f) * _speed * deltaTime;

            if (_t >= 1f) { _t = 1f; _forward = false; }
            if (_t <= 0f) { _t = 0f; _forward = true;  }

            Vector3 position = Vector3.Lerp(_pointA.position, _pointB.position, _t);

            // Write into snapshot — not cam.transform
            snapshot.Position = position;
            snapshot.Rotation = Quaternion.LookRotation(
                (_pointB.position - position).normalized
            );
        }
    }
}