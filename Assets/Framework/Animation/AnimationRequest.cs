namespace Framework.Animation
{
    public enum AnimationType
    {
        // =========================================
        // SHARED — 2D and 3D
        // =========================================
        Idle,
        Move,
        Attack,
        Hit,
        Death,
        Dodge,
        Stagger,

        // =========================================
        // 2D PLATFORMER
        // =========================================
        Jump,
        Fall,
        Land,
        WallSlide,
        Climb,
        Crouch,
        Dash
    }

    public struct AnimationRequest
    {
        public AnimationType Type;

        public AnimationRequest(AnimationType type)
        {
            Type = type;
        }
    }
}