using System;
using Levels;

namespace Data
{
    public class GameData
    {
        public LevelConfig CurrentLevel{ get; private set; }
        public int CurrentLevelIndex { get; private set; }
        public bool IsEnabledSound { get; private set; }

        public GameData()
        {
            IsEnabledSound = true;  
            CurrentLevelIndex = 1;
        }

        public void SetCurrentLevelIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            CurrentLevelIndex = index;
        }

        public void OpenNextLevel() => CurrentLevelIndex++;
        
        public void SetEnabledSound(bool enabled) => IsEnabledSound = enabled;
        
        public void SetCurrentLevel(LevelConfig levelConfig) => CurrentLevel = levelConfig;
    }
}