using UnityEngine;

namespace Gameplay.AI.Director
{
    public class AIDirector : MonoBehaviour
    {
        public static AIDirector Instance;

        public AIDirectorState State = new AIDirectorState();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            UpdateIntensity();
            UpdateSpawning();
        }

        private void UpdateIntensity()
        {
            // intensity grows over time
            State.Intensity += Time.deltaTime * 0.05f;

            // clamp
            State.Intensity = Mathf.Clamp01(State.Intensity);
        }

        private void UpdateSpawning()
        {
            State.TimeSinceLastWave += Time.deltaTime;

            // dynamic spawn timing
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