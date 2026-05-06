using UnityEngine;

namespace Gameplay.Camera
{
    // =========================================
    // CameraBootstrap2D
    // Configures the starting 2D camera mode.
    // Add to the same GameObject as
    // CameraModeController2D.
    // =========================================
    public class CameraBootstrap2D : MonoBehaviour
    {
        public enum StartingMode2D
        {
            Follow,
            Platformer,
            TopDown,
            Confiner
        }

        [Header("Mode")]
        public StartingMode2D startingMode = StartingMode2D.Follow;

        [Header("Target")]
        [Tooltip("GameObject to follow — finds Player tag if empty")]
        public Transform target;

        [Header("Follow Settings")]
        public float   smoothSpeed  = 5f;
        public float   zDepth       = -10f;
        public Vector2 offset       = Vector2.zero;

        [Header("Platformer Settings")]
        public float lookAheadDistance = 3f;
        public float yDamping          = 3f;
        public float deadZoneX         = 0.5f;
        public float deadZoneY         = 1f;

        [Header("Confiner Settings")]
        [Tooltip("Center of the camera bounds")]
        public Vector2 boundsCenter = Vector2.zero;
        [Tooltip("Size of the camera bounds")]
        public Vector2 boundsSize   = new Vector2(20f, 10f);

        [Header("Zoom")]
        public float orthographicSize = 5f;

        private void Start()
        {
            var controller = GetComponent<CameraModeController2D>();
            if (controller == null) return;

            // Find player if no target assigned
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    target = player.transform;
            }

            // Only modify camera if we have a valid 2D mode to set
            ICameraMode2D mode = BuildMode();
            if (mode == null) return;

            // Set orthographic — only do this if intentionally using 2D
            controller.cam.orthographic     = true;
            controller.cam.orthographicSize = orthographicSize;

            controller.SetMode(mode);
        }

        private ICameraMode2D BuildMode()
        {
            switch (startingMode)
            {
                case StartingMode2D.Follow:
                    return new Modes2D.Camera2DFollowMode(
                        target, smoothSpeed, zDepth, offset);

                case StartingMode2D.Platformer:
                    return new Modes2D.Camera2DPlatformerMode(
                        target, smoothSpeed, lookAheadDistance,
                        yDamping, deadZoneX, deadZoneY, zDepth);

                case StartingMode2D.TopDown:
                    return new Modes2D.Camera2DTopDownMode(
                        target, smoothSpeed, orthographicSize, zDepth);

                case StartingMode2D.Confiner:
                    return new Modes2D.Camera2DConfinerMode(
                        target,
                        new Bounds(
                            new Vector3(boundsCenter.x, boundsCenter.y, 0f),
                            new Vector3(boundsSize.x, boundsSize.y, 100f)),
                        smoothSpeed, zDepth);

                default:
                    return null;
            }
        }
    }
}