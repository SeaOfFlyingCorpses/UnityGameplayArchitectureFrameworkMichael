using System.Collections.Generic;

namespace Framework.Progression
{
    // =========================================
    // IClassSystem
    // Manages the player's class and evolution.
    //
    // A class defines:
    //   - Stat bonuses per level
    //   - Available abilities
    //   - Evolution paths (branching)
    //
    // Register as service:
    //   ServiceLocator.Register<IClassSystem>(this);
    // =========================================
    public interface IClassSystem
    {
        ICharacterClass           Current     { get; }
        IReadOnlyList<ICharacterClass> Available { get; }

        void SelectClass  (string classId);
        void EvolveClass  (string classId);
        bool CanEvolve    (string classId);
    }
}
