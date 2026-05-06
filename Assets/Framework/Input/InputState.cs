using UnityEngine;

namespace Framework.Input
{
    public class InputState
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool    AttackPressed;   // left click / face button
        public bool    PausePressed;    // Escape / Start
        public bool    JumpPressed;     // Space / South button
        public bool    DashPressed;     // Shift / East button
        public bool    InteractPressed; // E / West button
        public bool    CrouchPressed;   // C / Right stick click
    }
}