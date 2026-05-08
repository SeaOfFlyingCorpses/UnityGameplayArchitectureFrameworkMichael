namespace Framework.Events.Events.Gameplay
{
    public struct StageStartedEvent
    {
        public int StageNumber;
        public int WaveCount;

        public StageStartedEvent(int stage, int waves)
        {
            StageNumber = stage;
            WaveCount   = waves;
        }
    }

    public struct WaveStartedEvent
    {
        public int WaveNumber;
        public int EnemyCount;

        public WaveStartedEvent(int wave, int count)
        {
            WaveNumber  = wave;
            EnemyCount  = count;
        }
    }

    public struct WaveCompletedEvent
    {
        public int WaveNumber;
        public WaveCompletedEvent(int wave) { WaveNumber = wave; }
    }

    public struct StageCompletedEvent
    {
        public int StageNumber;
        public StageCompletedEvent(int stage) { StageNumber = stage; }
    }

    public struct BossSpawnedEvent
    {
        public UnityEngine.GameObject Boss;
        public BossSpawnedEvent(UnityEngine.GameObject boss) { Boss = boss; }
    }
}

namespace Framework.Events.Events.Gameplay
{
    public struct RespawnEvent
    {
        public UnityEngine.Transform Player;
        public UnityEngine.Vector3   Position;
        public int                   LivesRemaining;

        public RespawnEvent(
            UnityEngine.Transform player,
            UnityEngine.Vector3   position,
            int                   lives)
        {
            Player         = player;
            Position       = position;
            LivesRemaining = lives;
        }
    }

    public struct GameOverEvent { }

    public struct CheckpointReachedEvent
    {
        public UnityEngine.Transform Checkpoint;
        public CheckpointReachedEvent(UnityEngine.Transform cp)
        { Checkpoint = cp; }
    }
}

namespace Framework.Events.Events.Gameplay
{
    public struct ComboUpdatedEvent
    {
        public int   Combo;
        public float Multiplier;

        public ComboUpdatedEvent(int combo, float multiplier)
        {
            Combo      = combo;
            Multiplier = multiplier;
        }
    }

    public struct ComboResetEvent { }
}

namespace Framework.Events.Events.Gameplay
{
    public struct CurrencyChangedEvent
    {
        public string CurrencyId;
        public int    Delta;
        public int    NewTotal;

        public CurrencyChangedEvent(
            string currencyId, int delta, int newTotal)
        {
            CurrencyId = currencyId;
            Delta      = delta;
            NewTotal   = newTotal;
        }
    }
}
