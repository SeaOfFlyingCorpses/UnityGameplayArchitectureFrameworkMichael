using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Camera
{
    public class CameraModeController : MonoBehaviour
    {
        public UnityEngine.Camera cam;

        private ICameraMode _current;
        private ICameraMode _previous;

        private CameraSnapshot _currentSnapshot;
        private CameraSnapshot _previousSnapshot;

        private float _blendT;
        private float _blendTime;

        private readonly List<CameraRequest> _requests = new();
        private readonly Stack<ICameraMode> _cameraStack = new();

        // =========================================
        // REQUEST
        // =========================================
        public void Request(CameraRequest request)
        {
            _requests.Add(request);
        }

        private CameraRequest? Resolve()
        {
            if (_requests.Count == 0)
                return null;

            CameraRequest best = _requests[0];

            for (int i = 1; i < _requests.Count; i++)
            {
                if (_requests[i].Priority >= best.Priority)
                    best = _requests[i];
            }

            _requests.Clear();
            return best;
        }

        // =========================================
        // UPDATE
        // =========================================
        private void LateUpdate()
        {
            var request = Resolve();

            if (request.HasValue)
                SwitchMode(request.Value.Mode, request.Value.BlendTime);

            // base snapshot starts from camera
            _currentSnapshot = CameraSnapshot.Capture(cam);

            // APPLY CURRENT MODE
            _current?.Tick(cam, Time.deltaTime, ref _currentSnapshot);

            // STACK EFFECTS (shake, recoil, etc)
               foreach (var effect in _cameraStack)
            {
             //  effect?.Tick(cam, Time.deltaTime, ref _currentSnapshot);
            }

            // BLENDING
            if (_blendT < _blendTime && _previous != null)
            {
                _blendT += Time.deltaTime;
                float t = Mathf.Clamp01(_blendT / _blendTime);

                var blended = CameraSnapshot.Lerp(
                    _previousSnapshot,
                    _currentSnapshot,
                    t
                );

                blended.Apply(cam);
            }
            else
            {
                _currentSnapshot.Apply(cam);
            }
        }

        // =========================================
        // SWITCH MODE
        // =========================================
        private void SwitchMode(ICameraMode next, float blendTime)
        {
            if (next == _current)
                return;

            _previous?.Deactivate(cam);

            _previous = _current;
            _previousSnapshot = CameraSnapshot.Capture(cam);

            _current = next;
            _current?.Activate(cam);

            _blendTime = blendTime;
            _blendT = 0f;
        }

        // =========================================
        // STACK EFFECTS
        // =========================================
        public void AddEffectToStack(ICameraMode effect)
        {
            _cameraStack.Push(effect);
        }

        public void RemoveEffectFromStack(ICameraMode effect)
        {
            if (_cameraStack.Count > 0)
                _cameraStack.Pop();
        }
    }
}