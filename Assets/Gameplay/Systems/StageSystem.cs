using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Progression;

namespace Gameplay.Systems
{
    // =========================================
    // StageSystem
    // Manages wave spawning and stage progression.
    // Place on _GameSystems.
    //
    // Flow:
    //   StartStage() → Wave 1 spawns
    //   All wave enemies die → Wave 2 spawns
    //   ...
    //   Last wave clear → Boss spawns (if set)
    //   Boss dies → StageCompletedEvent fires
    //   XP and loot rewarded automatically
    //
    // Events fired:
    //   StageStartedEvent
    //   WaveStartedEvent
    //   WaveCompletedEvent
    //   BossSpawnedEvent
    //   StageCompletedEvent
    // =========================================
    public class StageSystem : MonoBehaviour
    {
        [Header("Stage")]
        public StageData currentStage;

        [Header("Settings")]
        public bool autoStartOnPlay = false;

        private int              _currentWave;
        private int              _activeEnemies;
        private bool             _stageActive;
        private bool             _bossAlive;
        private GameObject       _bossInstance;

        private void Awake()
        {
            ServiceLocator.Register<StageSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<StageSystem>();
        }

        private void Start()
        {
            if (autoStartOnPlay && currentStage != null)
                StartStage(currentStage);
        }

        // =========================================
        // PUBLIC API
        // =========================================
        public void StartStage(StageData stage)
        {
            if (_stageActive) return;

            currentStage  = stage;
            _currentWave  = 0;
            _stageActive  = true;
            _activeEnemies = 0;

            EventBus.Publish(new StageStartedEvent(
                stage.stageNumber,
                stage.waves.Count));

            StartCoroutine(RunStage());
        }

        public void ForceCompleteStage()
        {
            StopAllCoroutines();
            OnStageComplete();
        }

        // =========================================
        // STAGE RUNNER
        // =========================================
        private IEnumerator RunStage()
        {
            for (int i = 0; i < currentStage.waves.Count; i++)
            {
                _currentWave = i;
                var wave = currentStage.waves[i];

                yield return new WaitForSeconds(wave.startDelay);

                yield return StartCoroutine(RunWave(wave, i + 1));
            }

            OnStageComplete();
        }

        private IEnumerator RunWave(WaveData wave, int waveNumber)
        {
            _activeEnemies = 0;

            // Count total enemies
            foreach (var entry in wave.enemies)
                _activeEnemies += entry.count;

            EventBus.Publish(new WaveStartedEvent(
                waveNumber, _activeEnemies));

            // Spawn enemies with delays
            foreach (var entry in wave.enemies)
            {
                if (entry.spawnDelay > 0f)
                    yield return new WaitForSeconds(entry.spawnDelay);

                SpawnEnemyGroup(entry);
            }

            // Wait until all enemies are dead
            while (_activeEnemies > 0)
                yield return null;

            EventBus.Publish(new WaveCompletedEvent(waveNumber));

            // Spawn boss if this is boss wave
            if (wave.isBossWave && wave.bossPrefab != null)
            {
                yield return StartCoroutine(RunBoss(wave));
            }
        }

        private IEnumerator RunBoss(WaveData wave)
        {
            Vector3 pos = wave.bossSpawnPoint != null
                ? wave.bossSpawnPoint.position
                : transform.position;

            _bossInstance = Instantiate(wave.bossPrefab, pos,
                                         Quaternion.identity);
            _bossAlive    = true;

            // Subscribe to boss death
            var bossHealth = _bossInstance
                .GetComponent<Gameplay.Systems.Health.HealthComponent>();

            if (bossHealth != null)
                bossHealth.OnDeath += OnBossDeath;

            EventBus.Publish(new BossSpawnedEvent(_bossInstance));

            // Wait until boss dies
            while (_bossAlive)
                yield return null;
        }

        private void SpawnEnemyGroup(EnemySpawnEntry entry)
        {
            var registry = ServiceLocator.Get<PoolRegistry>();

            for (int i = 0; i < entry.count; i++)
            {
                if (entry.prefab == null) continue;

                Vector3 pos = transform.position;

                if (entry.spawnPoints != null &&
                    entry.spawnPoints.Length > 0)
                    pos = entry.spawnPoints[
                        i % entry.spawnPoints.Length].position;

                GameObject enemy;

                if (registry != null &&
                    registry.HasPool(entry.prefab.name))
                    enemy = registry.Get(entry.prefab.name,
                                         pos, Quaternion.identity);
                else
                    enemy = Instantiate(entry.prefab, pos,
                                         Quaternion.identity);

                // Track enemy death
                var health = enemy
                    .GetComponent<Gameplay.Systems.Health.HealthComponent>();

                if (health != null)
                    health.OnDeath += OnEnemyDeath;
            }
        }

        private void OnEnemyDeath()
        {
            _activeEnemies--;
            if (_activeEnemies < 0) _activeEnemies = 0;
        }

        private void OnBossDeath()
        {
            _bossAlive = false;
        }

        private void OnStageComplete()
        {
            _stageActive = false;

            // Grant XP reward
            var levelSystem = ServiceLocator.Get<ILevelSystem>();
            if (levelSystem != null && currentStage.xpReward > 0)
                levelSystem.AddExperience(currentStage.xpReward);

            EventBus.Publish(new StageCompletedEvent(
                currentStage.stageNumber));
        }
    }
}
