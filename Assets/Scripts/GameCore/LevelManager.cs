using UnityEngine;

namespace GameCore
{
    public static class LevelManager
    {
        public static LevelState CurrentLevelState = LevelState.Playing;

        public static int CurrentScore = 0;
        public static int CurrentLevel = 1;
        public static int CurrentLife = 3;

        public static bool CanRevive => CurrentLife > 0;
        public static Transform LatestCheckPoint;
        
        public static void InitializeLevel()
        {
            CurrentLevelState = LevelState.Playing;
            CurrentScore = 0;
            CurrentLevel = 1;
            CurrentLife = 3;
            LatestCheckPoint = null;
        }
        
        public enum LevelState
        {
            Waiting,
            Playing,
            Pause,
            GameOver,
            Win,
        }
    }
}
