using UnityEngine;

namespace Gameplay.Camera.Modes2D
{
    // =========================================
    // Camera2DConfinerMode
    // Follows target but stays within bounds.
    // Camera never shows outside the level.
    // Essential for tiled/room-based games.
    //
    // Usage:
    //   new Camera2DConfinerMode(
    //       target: player.transform,
    //       bounds: new Bounds(center, size))
    // =========================================
    public class Camera2DConfinerMode : ICameraMode2D
    {
        private readonly Transform _target;
        private readonly Bounds    _bounds;
        private readonly float     _smoothSpeed;
        private readonly float     _zDepth;

        public Camera2DConfinerMode(
            Transform target,
            Bounds    bounds,
            float     smoothSpeed = 5f,
            float     zDepth      = -10f)
        {
            _target      = target;
            _bounds      = bounds;
            _smoothSpeed = smoothSpeed;
            _zDepth      = zDepth;
        }

        public void Activate(UnityEngine.Camera cam) { }
        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float dt, ref CameraSnapshot2D s)
        {
            if (_target == null) return;

            // Follow target
            Vector3 desired = new Vector3(
                _target.position.x,
                _target.position.y,
                _zDepth);

            // Clamp to bounds accounting for camera view size
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            desired.x = Mathf.Clamp(
                desired.x,
                _bounds.min.x + halfW,
                _bounds.max.x - halfW);

            desired.y = Mathf.Clamp(
                desired.y,
                _bounds.min.y + halfH,
                _bounds.max.y - halfH);

            s.Position = Vector3.Lerp(
                s.Position, desired, dt * _smoothSpeed);
        }
    }
}