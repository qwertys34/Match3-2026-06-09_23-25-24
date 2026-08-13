using System;
using Audio;
using Cysharp.Threading.Tasks;
using Data;
using Save;
using SceneLoading;
using UnityEngine.UI;

namespace Game.Score
{
    public class EndGame
    {
        private AudioManager _audioManager;
        private IAsyncSceneLoading sceneLoading;
        private GameData _gameData;
        private SaveProgress _saveProgress;

        public EndGame(AudioManager audioManager, IAsyncSceneLoading sceneLoading, GameData gameData,
            SaveProgress saveProgress)
        {
            _audioManager = audioManager;
            this.sceneLoading = sceneLoading;
            _gameData = gameData;
            _saveProgress = saveProgress;
        }

        public async void End(bool success, Button button)
        {
            if (success && _gameData.CurrentLevel.LevelNumber == _gameData.CurrentLevelIndex) 
                _gameData.OpenNextLevel();
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            button.interactable = true;
            _saveProgress.Save();
            _audioManager.StopMusic();
            await sceneLoading.UnloadAsync(Scenes.GAME);
            await sceneLoading.LoadAsync(Scenes.MENU);
            _audioManager.PlayMenuMusic();
        }
        
    }
}