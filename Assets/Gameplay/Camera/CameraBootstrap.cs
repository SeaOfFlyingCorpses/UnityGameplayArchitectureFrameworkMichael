using UnityEngine;
using Framework.Input;
using Gameplay.Input;
using Gameplay.Camera.Modes;

namespace Gameplay.Camera
{
    public class CameraBootstrap : MonoBehaviour
    {
        public enum StartingMode
        {
            FreeLook,
            FPS,
            ThirdPerson,
            OverShoulder,
            TopDown,
            Isometric,
            Orbit,
            Fixed,
            Cinematic
        }

        [Header("Mode")]
        public StartingMode mode = StartingMode.FreeLook;

        [Header("References")]
        public CameraModeController controller;
        public Transform            playerTarget;
        public InputHandler         inputHandler;

        [Header("Camera Tuning")]
        [Tooltip("Distance from target (FreeLook, ThirdPerson, Orbit)")]
        public float cameraDistance = 5f;
        [Tooltip("Height above target")]
        public float cameraHeight   = 3f;

        [Header("Fixed Mode")]
        public Vector3    fixedPosition;
        public Transform  fixedLookAt;

        [Header("Cinematic Mode")]
        public Transform cinematicPointA;
        public Transform cinematicPointB;

        [Header("Settings")]
        public CameraPriority priority  = CameraPriority.Normal;
        public float          blendTime = 0f;

        private void Start()
        {
            if (controller == null)
            {
                Debug.LogWarning("CameraBootstrap: Controller not assigned.");
                return;
            }

            ICameraMode cameraMode = BuildMode();

            if (cameraMode == null)
                return;

            controller.Request(new CameraRequest(cameraMode, priority, blendTime));
        }

        private ICameraMode BuildMode()
        {
            switch (mode)
            {
                case StartingMode.FreeLook:
                    if (playerTarget == null || inputHandler == null)
                    {
                        Debug.LogWarning("CameraBootstrap: FreeLook needs PlayerTarget and InputHandler.");
                        return null;
                    }
                    return new FreeLookCameraMode(playerTarget, inputHandler.State, cameraDistance, cameraHeight);

                case StartingMode.FPS:
                    if (playerTarget == null)
                    {
                        Debug.LogWarning("CameraBootstrap: FPS needs PlayerTarget (head transform).");
                        return null;
                    }
                    return new FPSCameraMode(playerTarget);

                case StartingMode.ThirdPerson:
                    if (playerTarget == null || inputHandler == null)
                    {
                        Debug.LogWarning("CameraBootstrap: ThirdPerson needs PlayerTarget and InputHandler.");
                        return null;
                    }
                    return new ThirdPersonCameraMode(playerTarget, inputHandler.State);

                case StartingMode.OverShoulder:
                    if (playerTarget == null || inputHandler == null)
                    {
                        Debug.LogWarning("CameraBootstrap: OverShoulder needs PlayerTarget and InputHandler.");
                        return null;
                    }
                    return new OverShoulderCameraMode(playerTarget, inputHandler.State);

                case StartingMode.TopDown:
                    if (playerTarget == null)
                    {
                        Debug.LogWarning("CameraBootstrap: TopDown needs PlayerTarget.");
                        return null;
                    }
                    return new TopDownCameraMode(playerTarget);

                case StartingMode.Isometric:
                    if (playerTarget == null)
                    {
                        Debug.LogWarning("CameraBootstrap: Isometric needs PlayerTarget.");
                        return null;
                    }
                    return new IsometricCameraMode(playerTarget);

                case StartingMode.Orbit:
                    if (playerTarget == null)
                    {
                        Debug.LogWarning("CameraBootstrap: Orbit needs PlayerTarget.");
                        return null;
                    }
                    return new OrbitCameraMode(playerTarget, input: inputHandler?.State);

                case StartingMode.Fixed:
                    return new FixedCameraMode(fixedPosition, fixedLookAt);

                case StartingMode.Cinematic:
                    if (cinematicPointA == null || cinematicPointB == null)
                    {
                        Debug.LogWarning("CameraBootstrap: Cinematic needs PointA and PointB.");
                        return null;
                    }
                    return new CinematicCameraMode(cinematicPointA, cinematicPointB);

                default:
                    Debug.LogWarning($"CameraBootstrap: Unknown mode {mode}.");
                    return null;
            }
        }
    }
}