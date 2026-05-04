namespace Framework.AI.Lifecycle
{
    public interface IAIEntity
    {
        void Initialize();
        void OnDeath();
        void Dispose();
    }
}