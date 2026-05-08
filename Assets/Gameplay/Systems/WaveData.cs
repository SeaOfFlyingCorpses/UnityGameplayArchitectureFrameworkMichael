using System.Collections.Generic;
using Gameplay.Items;
using UnityEngine;

namespace Gameplay.Systems
{
    // =========================================
    // EnemySpawnEntry
    // One enemy type in a wave.
    // =========================================
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject  prefab;
        public int         count     = 3;
        public Transform[] spawnPoints;
        public float       spawnDelay = 0f;
    }

    // =========================================
    // WaveData
    // Defines one wave of enemies.
    // =========================================
    [System.Serializable]
    public class WaveData
    {
        [Tooltip("Enemies to spawn in this wave")]
        public List<EnemySpawnEntry> enemies = new();

        [Tooltip("Delay before wave starts")]
        public float startDelay = 1f;

        [Tooltip("Is this the boss wave?")]
        public bool  isBossWave = false;

        [Tooltip("Boss prefab — spawned after all enemies die")]
        public GameObject bossPrefab;
        public Transform  bossSpawnPoint;
    }

    // =========================================
    // StageData
    // ScriptableObject defining a full stage.
    //
    // Create:
    //   Right click → Create → Gameplay → Stage
    // =========================================
    [CreateAssetMenu(
        fileName = "NewStage",
        menuName  = "Gameplay/Stage")]
    public class StageData : ScriptableObject
    {
        [Header("Stage Info")]
        public int    stageNumber = 1;
        public string stageName   = "Stage 1";

        [Header("Waves")]
        public List<WaveData> waves = new();

        [Header("Rewards")]
        public int         xpReward       = 500;
        public LootTable   completionLoot;
    }
}
