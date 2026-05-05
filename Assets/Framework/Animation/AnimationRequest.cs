namespace Framework.Animation
{
    public enum AnimationType
    {
        Idle,
        Move,
        Attack,
        Hit,
        Death,    // plays death animation, sets IsDead bool
        Dodge,    // plays dodge/roll animation
        Stagger   // plays hit stagger animation
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