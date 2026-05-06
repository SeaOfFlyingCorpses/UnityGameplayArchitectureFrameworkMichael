using UnityEngine;

namespace Gameplay.Camera.Modes2D
{
    // =========================================
    // Camera2DTopDownMode
    // Top-down 2D follow.
    // Follows target in both X and Y.
    // Optional zoom based on speed.
    // =========================================
    public class Camera2DTopDownMode : ICameraMode2D
    {
        private readonly Transform _target;
        private readonly float     _smoothSpeed;
        private readonly float     _baseOrthoSize;
        private readonly float     _zDepth;

        public Camera2DTopDownMode(
            Transform target,
            float     smoothSpeed   = 5f,
            float     baseOrthoSize = 5f,
            float     zDepth        = -10f)
        {
            _target        = target;
            _smoothSpeed   = smoothSpeed;
            _baseOrthoSize = baseOrthoSize;
            _zDepth        = zDepth;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            cam.orthographic     = true;
            cam.orthographicSize = _baseOrthoSize;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float dt, ref CameraSnapshot2D s)
        {
            if (_target == null) return;

            Vector3 targetPos = new Vector3(
                _target.position.x,
                _target.position.y,
                _zDepth);

            s.Position  = Vector3.Lerp(
                s.Position, targetPos, dt * _smoothSpeed);
            s.OrthoSize = _baseOrthoSize;
        }
    }
}