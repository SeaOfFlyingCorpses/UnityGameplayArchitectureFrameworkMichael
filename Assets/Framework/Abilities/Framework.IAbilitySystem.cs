namespace Framework.Abilities
{
    public interface IAbilitySystem
    {
        void Use(string abilityId, object context);
        void Register(object ability);
    }
}