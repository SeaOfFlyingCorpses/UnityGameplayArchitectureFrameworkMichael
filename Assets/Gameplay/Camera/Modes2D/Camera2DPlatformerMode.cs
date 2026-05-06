using UnityEngine;

namespace Gameplay.Camera.Modes2D
{
    // =========================================
    // Camera2DPlatformerMode
    // Side-scrolling platformer camera.
    // Leads ahead in movement direction.
    // Snaps Y loosely — doesn't follow every jump.
    // Dead zone — camera only moves when
    // target leaves the center zone.
    // =========================================
    public class Camera2DPlatformerMode : ICameraMode2D
    {
        private readonly Transform _target;
        private readonly float     _smoothSpeed;
        private readonly float     _lookAheadDistance;
        private readonly float     _yDamping;
        private readonly float     _deadZoneX;
        private readonly float     _deadZoneY;
        private readonly float     _zDepth;

        private float _lookAheadX;
        private float _lastTargetX;

        public Camera2DPlatformerMode(
            Transform target,
            float     smoothSpeed        = 6f,
            float     lookAheadDistance  = 3f,
            float     yDamping           = 3f,
            float     deadZoneX          = 0.5f,
            float     deadZoneY          = 1f,
            float     zDepth             = -10f)
        {
            _target            = target;
            _smoothSpeed       = smoothSpeed;
            _lookAheadDistance = lookAheadDistance;
            _yDamping          = yDamping;
            _deadZoneX         = deadZoneX;
            _deadZoneY         = deadZoneY;
            _zDepth            = zDepth;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            if (_target != null)
                _lastTargetX = _target.position.x;
        }

        public void Deactivate(UnityEngine.Camera cam) { }

        public void Tick(UnityEngine.Camera cam, float dt, ref CameraSnapshot2D s)
        {
            if (_target == null) return;

            float targetX = _target.position.x;
            float targetY = _target.position.y;

            // Look ahead in movement direction
            float deltaX = targetX - _lastTargetX;
            if (Mathf.Abs(deltaX) > 0.01f)
                _lookAheadX = Mathf.Lerp(
                    _lookAheadX,
                    _lookAheadDistance * Mathf.Sign(deltaX),
                    dt * _smoothSpeed);

            _lastTargetX = targetX;

            // Dead zone X
            float currentX = s.Position.x;
            float diffX    = (targetX + _lookAheadX) - currentX;
            float newX     = Mathf.Abs(diffX) > _deadZoneX
                ? currentX + diffX * dt * _smoothSpeed
                : currentX;

            // Dead zone Y — loose vertical follow
            float currentY = s.Position.y;
            float diffY    = targetY - currentY;
            float newY     = Mathf.Abs(diffY) > _deadZoneY
                ? Mathf.Lerp(currentY, targetY, dt * _yDamping)
                : currentY;

            s.Position = new Vector3(newX, newY, _zDepth);
        }
    }
}