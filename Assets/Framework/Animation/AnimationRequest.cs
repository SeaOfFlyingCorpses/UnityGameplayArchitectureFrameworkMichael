namespace Framework.Animation
{
    public enum AnimationType
    {
        Idle,
        Move,
        Attack,
        Hit
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