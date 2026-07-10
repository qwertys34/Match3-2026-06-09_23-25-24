using System;
using Audio;
using Cysharp.Threading.Tasks;
using Data;
using SceneLoading;
using UnityEngine.UI;

namespace Game.Score
{
    public class EndGame
    {
        private AudioManager _audioManager;
        private IAsyncSceneLoading sceneLoading;
        private GameData _gameData;

        public EndGame(AudioManager audioManager, IAsyncSceneLoading sceneLoading, GameData gameData)
        {
            _audioManager = audioManager;
            this.sceneLoading = sceneLoading;
            _gameData = gameData;
        }

        public async void End(bool success, Button button)
        {
            if (success && _gameData.CurrentLevel.LevelNumber == _gameData.CurrentLevelIndex) 
                _gameData.OpenNextLevel();
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            button.interactable = true;
            _audioManager.StopMusic();
            await sceneLoading.UnloadAsync(Scenes.GAME);
            await sceneLoading.LoadAsync(Scenes.MENU);
            _audioManager.PlayMenuMusic();
        }
    }
}