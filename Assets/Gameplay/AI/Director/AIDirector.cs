using UnityEngine;
using Framework.Core;

namespace Gameplay.AI.Director
{
    public class AIDirector : MonoBehaviour
    {
        public AIDirectorState State = new AIDirectorState();

        // =========================================
        // SPAWN ENTRY
        // Each entry is one pool type with its own
        // spawn weight and spawn points.
        // Add as many as you need in the Inspector.
        // =========================================
        [System.Serializable]
        public class SpawnEntry
        {
            [Tooltip("Must match a key registered in PoolRegistry (e.g. 'Enemy', 'EliteEnemy')")]
            public string      poolKey;

            [Tooltip("Relative weight — higher = spawns more often")]
            public float       weight = 1f;

            [Tooltip("Spawn points for this type — picked randomly")]
            public Transform[] spawnPoints;
        }

        [Header("Spawning")]
        public SpawnEntry[] spawnEntries;

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

            if (registry == null || spawnEntries == null || spawnEntries.Length == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                if (State.ActiveEnemies >= State.MaxEnemies)
                    break;

                var entry = PickEntry();

                if (entry == null)
                    continue;

                if (!registry.HasPool(entry.poolKey))
                    continue;

                if (entry.spawnPoints == null || entry.spawnPoints.Length == 0)
                {
                    Debug.LogWarning($"[AIDirector] SpawnEntry '{entry.poolKey}' has no spawn points.");
                    continue;
                }

                var point = entry.spawnPoints[Random.Range(0, entry.spawnPoints.Length)];
                var obj   = registry.Get(entry.poolKey, point.position, point.rotation);

                if (obj != null)
                    State.ActiveEnemies++;
            }
        }

        // =========================================
        // WEIGHTED RANDOM PICK
        // Higher weight = more likely to be chosen
        // =========================================
        private SpawnEntry PickEntry()
        {
            float total = 0f;

            foreach (var entry in spawnEntries)
                total += entry.weight;

            float roll = Random.Range(0f, total);
            float cumulative = 0f;

            foreach (var entry in spawnEntries)
            {
                cumulative += entry.weight;
                if (roll <= cumulative)
                    return entry;
            }

            return spawnEntries[spawnEntries.Length - 1];
        }
    }
}