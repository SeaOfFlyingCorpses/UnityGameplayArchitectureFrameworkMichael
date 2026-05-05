using UnityEngine;

namespace Gameplay.Camera
{
    // =========================================
    // CameraShakeEffect
    // Stacks on top of any camera mode via
    // CameraModeController.AddEffectToStack().
    // Writes into snapshot — not cam.transform.
    //
    // Usage:
    //   var shake = new CameraShakeEffect(0.3f, 0.5f);
    //   controller.AddEffectToStack(shake);
    // =========================================
    public class CameraShakeEffect : ICameraMode
    {
        private float   _intensity;
        private float   _duration;
        private float   _remaining;

        public bool IsFinished => _remaining <= 0f;

        public CameraShakeEffect(float intensity, float duration)
        {
            _intensity = intensity;
            _duration  = duration;
            _remaining = duration;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _remaining = _duration;
        }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_remaining <= 0f)
                return;

            _remaining -= deltaTime;

            // Fade intensity as duration runs out
            float fade = Mathf.Clamp01(_remaining / _duration);

            snapshot.Position += Random.insideUnitSphere * _intensity * fade;
        }

        public void Deactivate(UnityEngine.Camera cam) { }
    }
}