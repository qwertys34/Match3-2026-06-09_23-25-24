using Data;
using UnityEngine;

namespace Save
{
    public class SaveProgress 
    {
        private GameData _gameData;

        public SaveProgress(GameData gameData)
        {
            _gameData = gameData;
        }

        private const string CurrentLevel = "level";
        private const string IsEnableSound = "sound";

        public void Save()
        {
            PlayerPrefs.SetInt(CurrentLevel, _gameData.CurrentLevelIndex);
            PlayerPrefs.SetInt(IsEnableSound, _gameData.IsEnabledSound ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (PlayerPrefs.GetInt(CurrentLevel) > 1)
                _gameData.SetCurrentLevelIndex(PlayerPrefs.GetInt(CurrentLevel));
            else _gameData.SetCurrentLevelIndex(1);
            
            _gameData.SetEnabledSound(PlayerPrefs.GetInt(IsEnableSound) == 1);
        }
        
        public void ResetProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Reset all data progress");
        }
        
    }
}