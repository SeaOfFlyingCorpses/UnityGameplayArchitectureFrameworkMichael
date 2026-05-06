using UnityEngine;
using Framework.Core;
using Framework.AI.Squad;
using Gameplay.AI.Squad;
using Gameplay.AI.Formation;

namespace Gameplay.Bootstrap
{
    // =========================================
    // SquadFormationSetup
    // Assigns formations to each squad on Start.
    // Place on _GameSystems.
    //
    // Without this, formations are null and
    // FormationAISystem does nothing (by design).
    // =========================================
    public class SquadFormationSetup : MonoBehaviour
    {
        [Header("Enemy Formation")]
        public bool          enableEnemyFormation = true;
        public FormationType enemyFormationType   = FormationType.Wedge;
        public float         enemySpacing         = 2.5f;

        [Header("Ally Formation")]
        public bool          enableAllyFormation  = true;
        public FormationType allyFormationType    = FormationType.Line;
        public float         allySpacing          = 2f;

        [Header("Player Squad Formation")]
        public bool          enablePlayerFormation = false;
        public FormationType playerFormationType   = FormationType.Wedge;
        public float         playerSpacing         = 2f;

        private void Start()
        {
            var squadSystem = ServiceLocator.Get<SquadSystem>();

            if (squadSystem == null)
            {
                Debug.LogWarning("[SquadFormationSetup] No SquadSystem found.");
                return;
            }

            if (enableEnemyFormation)
                squadSystem.EnemySquad.Formation =
                    new FormationData(enemyFormationType)
                    {
                        Spacing = enemySpacing
                    };

            if (enableAllyFormation)
                squadSystem.AllySquad.Formation =
                    new FormationData(allyFormationType)
                    {
                        Spacing = allySpacing
                    };

            if (enablePlayerFormation)
                squadSystem.PlayerSquad.Formation =
                    new FormationData(playerFormationType)
                    {
                        Spacing = playerSpacing
                    };
        }
    }
}
