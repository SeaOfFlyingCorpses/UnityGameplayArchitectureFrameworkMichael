using UnityEngine;
using Framework.Input;

namespace Gameplay.Player
{
    // =========================================
    // PlayerLook
    // Rotates the player body (yaw) and head
    // (pitch) using mouse input from InputHandler.
    // FPSCameraMode follows Head.rotation directly.
    //
    // Attach to Player GameObject.
    // Assign Head transform and InputHandler.
    // =========================================
    public class PlayerLook : MonoBehaviour
    {
        [Header("References")]
        public Transform    head;
        public InputHandler inputHandler;

        [Header("Settings")]
        public float sensitivity = 120f;

        [Range(-90f, 0f)]
        public float pitchMin = -80f;

        [Range(0f, 90f)]
        public float pitchMax =  80f;

        private float _pitch;
        private float _yaw;

        private void Start()
        {
            _yaw   = transform.eulerAngles.y;
            _pitch = 0f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        private void Update()
        {
            if (inputHandler == null || head == null)
                return;

            var look = inputHandler.State.Look;

            // Horizontal — rotate the whole player body (yaw)
            _yaw   += look.x * sensitivity * Time.deltaTime;

            // Vertical — rotate only the head (pitch)
            _pitch -= look.y * sensitivity * Time.deltaTime;
            _pitch  = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            // Apply
            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}