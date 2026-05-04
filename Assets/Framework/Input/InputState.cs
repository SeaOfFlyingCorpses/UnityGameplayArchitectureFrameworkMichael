using UnityEngine;

namespace Framework.Input
{
    public class InputState
    {
        public Vector2 Move;
        public Vector2 Look;          // mouse delta / right stick
        public bool    AttackPressed;
    }
}