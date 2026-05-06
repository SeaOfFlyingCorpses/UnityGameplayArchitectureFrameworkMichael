using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Camera
{
    // =========================================
    // CameraModeController2D
    // Drives all 2D camera modes.
    // Same pattern as CameraModeController (3D).
    //
    // Setup:
    //   1. Add to Main Camera GameObject
    //   2. Make sure Camera is set to Orthographic
    //   3. Add CameraBootstrap2D to configure modes
    //
    // All modes write into CameraSnapshot2D.
    // Controller applies snapshot in LateUpdate.
    // Never let modes write to cam.transform directly.
    // =========================================
    public class CameraModeController2D : MonoBehaviour
    {
        public UnityEngine.Camera cam;

        private ICameraMode2D _current;
        private ICameraMode2D _previous;

        private CameraSnapshot2D _currentSnapshot;
        private CameraSnapshot2D _previousSnapshot;

        private float _blendT;
        private float _blendTime;

        private readonly List<CameraRequest2D> _requests = new();

        // =========================================
        // REQUEST
        // =========================================
        public void Request(CameraRequest2D request)
        {
            _requests.Add(request);
        }

        // =========================================
        // DIRECT SET — no blend
        // =========================================
        public void SetMode(ICameraMode2D mode)
        {
            SwitchMode(mode, 0f);
        }

        private void Awake()
        {
            if (cam == null)
                cam = GetComponent<UnityEngine.Camera>();

            if (cam != null)
                cam.orthographic = true;
        }

        private void LateUpdate()
        {
            // Process requests
            var request = Resolve();
            if (request.HasValue)
                SwitchMode(request.Value.Mode, request.Value.BlendTime);

            // Capture current state
            _currentSnapshot = CameraSnapshot2D.Capture(cam);

            // Tick current mode
            _current?.Tick(cam, Time.deltaTime, ref _currentSnapshot);

            // Blending
            if (_blendT < _blendTime && _previous != null)
            {
                _blendT += Time.deltaTime;
                float t = Mathf.Clamp01(_blendT / _blendTime);

                CameraSnapshot2D.Lerp(
                    _previousSnapshot,
                    _currentSnapshot,
                    t).Apply(cam);
            }
            else
            {
                _currentSnapshot.Apply(cam);
            }
        }

        private CameraRequest2D? Resolve()
        {
            if (_requests.Count == 0) return null;

            var best = _requests[0];
            for (int i = 1; i < _requests.Count; i++)
                if (_requests[i].Priority >= best.Priority)
                    best = _requests[i];

            _requests.Clear();
            return best;
        }

        private void SwitchMode(ICameraMode2D next, float blendTime)
        {
            if (next == _current) return;

            _previous?.Deactivate(cam);
            _previous         = _current;
            _previousSnapshot = CameraSnapshot2D.Capture(cam);

            _current = next;
            _current?.Activate(cam);

            _blendTime = blendTime;
            _blendT    = 0f;
        }
    }

    // =========================================
    // CAMERA REQUEST 2D
    // =========================================
    public struct CameraRequest2D
    {
        public ICameraMode2D Mode;
        public int           Priority;
        public float         BlendTime;
    }
}