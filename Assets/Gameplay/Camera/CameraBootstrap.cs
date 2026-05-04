using UnityEngine;
using Framework.Input;
using Gameplay.Camera.Modes;

namespace Gameplay.Camera
{
    // =========================================
    // CameraBootstrap
    // Pick the starting camera mode from the
    // Inspector — no code changes needed.
    // =========================================
    public class CameraBootstrap : MonoBehaviour
    {
        public enum StartingMode
        {
            FreeLook,
            FPS,
            Cinematic
        }

        [Header("Mode")]
        public StartingMode mode = StartingMode.FreeLook;

        [Header("References")]
        public CameraModeController controller;
        public Transform            playerTarget;
        public InputHandler         inputHandler;

        [Header("Cinematic Only")]
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

                    return new FreeLookCameraMode(playerTarget, inputHandler.State);

                case StartingMode.FPS:

                    if (playerTarget == null)
                    {
                        Debug.LogWarning("CameraBootstrap: FPS needs PlayerTarget (head transform).");
                        return null;
                    }

                    return new FPSCameraMode(playerTarget);

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