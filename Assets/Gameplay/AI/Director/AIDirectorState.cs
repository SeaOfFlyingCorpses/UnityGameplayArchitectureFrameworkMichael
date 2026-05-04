namespace Gameplay.AI.Director
{
    public class AIDirectorState
    {
        public float Intensity; 
        public float Difficulty; 

        public float TimeSinceLastWave;

        public int ActiveEnemies;
        public int MaxEnemies = 10;
    }
}