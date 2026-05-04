using UnityEngine;
using Framework.Core;

namespace Gameplay.AI.Director
{
    public class AIDirector : MonoBehaviour
    {
        // =========================================
        // No more "public static Instance"
        // Retrieve via: ServiceLocator.Get<AIDirector>()
        // =========================================

        public AIDirectorState State = new AIDirectorState();

        private void Awake()
        {
            ServiceLocator.Register<AIDirector>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<AIDirector>();
        }

        private void Update()
        {
            UpdateIntensity();
            UpdateSpawning();
        }

        private void UpdateIntensity()
        {
            State.Intensity += Time.deltaTime * 0.05f;
            State.Intensity  = Mathf.Clamp01(State.Intensity);
        }

        private void UpdateSpawning()
        {
            State.TimeSinceLastWave += Time.deltaTime;

            float spawnDelay = Mathf.Lerp(10f, 2f, State.Intensity);

            if (State.TimeSinceLastWave >= spawnDelay)
            {
                TrySpawnWave();
                State.TimeSinceLastWave = 0f;
            }
        }

        private void TrySpawnWave()
        {
            if (State.ActiveEnemies >= State.MaxEnemies)
                return;

            int spawnCount = Mathf.RoundToInt(
                Mathf.Lerp(1, 5, State.Intensity)
            );

            SpawnEnemies(spawnCount);
        }

        private void SpawnEnemies(int count)
        {
            // hook into your spawn system later
            Debug.Log($"Spawn {count} enemies");
            State.ActiveEnemies += count;
        }
    }
}