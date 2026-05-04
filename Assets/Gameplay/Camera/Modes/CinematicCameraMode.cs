using UnityEngine;

namespace Gameplay.Camera.Modes
{
    public class CinematicCameraMode : ICameraMode
    {
        private Transform _pointA;
        private Transform _pointB;
        private float _t;

        public CinematicCameraMode(Transform a, Transform b)
        {
            _pointA = a;
            _pointB = b;
        }

        public void Activate(UnityEngine.Camera cam)
        {
            _t = 0f;
        }

        public void Deactivate(UnityEngine.Camera cam)
        {
        }

        public void Tick(UnityEngine.Camera cam, float deltaTime, ref CameraSnapshot snapshot)
        {
            _t += deltaTime * 0.2f;

            cam.transform.position = Vector3.Lerp(_pointA.position, _pointB.position, _t);
            cam.transform.LookAt(_pointB.position);
        }
    }
}