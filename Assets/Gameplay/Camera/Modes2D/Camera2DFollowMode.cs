using UnityEngine;

namespace Gameplay.Camera.Modes2D
{
    // =========================================
    // Camera2DFollowMode
    // Smoothly follows a target in XY.
    // Standard 2D follow camera for any genre.
    //
    // Usage:
    //   new Camera2DFollowMode(player.transform)
    // =========================================
    public class Camera2DFollowMode : ICameraMode2D
    {
        private readonly Transform _target;
        private readonly float     _smoothSpeed;
        private readonly float     _zDepth;
        private readonly Vector2   _offset;

        public Camera2DFollowMode(
            Transform target,
            float     smoothSpeed = 5f,
            float     zDepth      = -10f,
            Vector2   offset      = default)
        {
            _target      = target;
            _smoothSpeed = smoothSpeed;
            _zDepth      = zDepth;
            _offset      = offset;
        }

        public void Activate(UnityEngine.Camera cam) { }
        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float dt, ref CameraSnapshot2D s)
        {
            if (_target == null) return;

            Vector3 targetPos = new Vector3(
                _target.position.x + _offset.x,
                _target.position.y + _offset.y,
                _zDepth);

            s.Position = Vector3.Lerp(
                s.Position, targetPos, dt * _smoothSpeed);
        }
    }
}