using UnityEngine;
using Framework.Core;

namespace Gameplay.AI.Director
{
    public class AIDirector : MonoBehaviour
    {
        public AIDirectorState State = new AIDirectorState();

        [Header("Spawning")]
        [Tooltip("Pool key to spawn enemies from. Must match a key in PoolRegistry.")]
        public string enemyPoolKey = "Enemy";

        [Tooltip("Spawn points — enemies appear at one of these randomly.")]
        public Transform[] spawnPoints;

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

            int count = Mathf.RoundToInt(Mathf.Lerp(1, 5, State.Intensity));
            SpawnEnemies(count);
        }

        private void SpawnEnemies(int count)
        {
            var registry = ServiceLocator.Get<PoolRegistry>();

            if (registry == null || !registry.HasPool(enemyPoolKey))
            {
                // Pool not set up yet — log once, don't spam
                return;
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("[AIDirector] No spawn points assigned.");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (State.ActiveEnemies >= State.MaxEnemies)
                    break;

                // Pick a random spawn point
                var point = spawnPoints[Random.Range(0, spawnPoints.Length)];

                var obj = registry.Get(
                    enemyPoolKey,
                    point.position,
                    point.rotation
                );

                if (obj != null)
                    State.ActiveEnemies++;
            }
        }
    }
}