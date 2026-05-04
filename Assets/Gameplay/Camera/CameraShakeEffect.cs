using UnityEngine;

namespace Gameplay.Camera
{
    public class CameraShakeEffect : ICameraMode
    {
        private float _intensity;
        private float _duration;
        private Vector3 _initialPosition;

        public CameraShakeEffect(float intensity, float duration)
        {
            _intensity = intensity;
            _duration = duration;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _initialPosition = cam.transform.position; // Store the initial camera position
        }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            if (_duration > 0)
            {
                // Apply random shake based on intensity
                cam.transform.position = _initialPosition + Random.insideUnitSphere * _intensity;
                _duration -= deltaTime; // Reduce duration over time
            }
        }

        public void Deactivate(UnityEngine.Camera cam)
        {
            // Reset camera position after shake ends
            cam.transform.position = _initialPosition;
        }
    }
}