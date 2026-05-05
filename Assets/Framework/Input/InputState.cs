using UnityEngine;

namespace Framework.Input
{
    public class InputState
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool    AttackPressed;   // mapped to left click / gamepad face button
        public bool    PausePressed;    // mapped to Escape / Start / Options
    }
}