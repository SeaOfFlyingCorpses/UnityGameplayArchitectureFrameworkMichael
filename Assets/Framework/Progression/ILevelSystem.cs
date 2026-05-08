namespace Framework.Progression
{
    // =========================================
    // ILevelSystem
    // Tracks experience and level for any entity.
    // Works for player, enemies, skills, weapons
    // — anything that can gain experience.
    //
    // Register as service for player:
    //   ServiceLocator.Register<ILevelSystem>(this);
    //
    // Or keep per-agent as a component.
    // =========================================
    public interface ILevelSystem
    {
        int   Level      { get; }
        int   Experience { get; }
        int   NextLevel  { get; } // XP required for next level
        float Progress   { get; } // 0-1 progress to next level

        void AddExperience (int amount);
        void SetLevel      (int level);
        bool IsMaxLevel    { get; }
    }
}
