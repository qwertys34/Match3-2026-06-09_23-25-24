using System;
using System.Threading;
using Audio;
using Cysharp.Threading.Tasks;
using Game.Tiles;
using Levels;
using SceneLoading;

namespace Data
{
    public class StartGame
    {
        private GameData _gameData;
        private AudioManager _audioManager;
        private IAsyncSceneLoading _asyncSceneLoading;
        private InteractablesTilesSetup _interactablesTilesSetup;
        private CancellationTokenSource _cts;
        
        public StartGame(GameData gameData, AudioManager audioManager, IAsyncSceneLoading asyncSceneLoading,
            InteractablesTilesSetup interactablesTilesSetup)
        {
            _gameData = gameData;
            _audioManager = audioManager;
            _asyncSceneLoading = asyncSceneLoading;
            _interactablesTilesSetup = interactablesTilesSetup;
        }

        public async UniTask Start(LevelConfig level)
        {
            _cts = new CancellationTokenSource();
            _gameData.SetCurrentLevel(level);
            _audioManager.StopMusic();
            _audioManager.PlayStopMusic();
            //await UniTask.Delay(TimeSpan.FromSeconds(1f)); 
            await _asyncSceneLoading.UnloadAsync(Scenes.MENU);
            await _asyncSceneLoading.LoadAsync(Scenes.GAME);
            _audioManager.PlayGameMusic();
            _cts.Cancel();
        }
        
        public async UniTask StartRandomLevel(LevelConfig level)
        {
            _cts = new CancellationTokenSource();
            await _gameData.SetRandomLevel(level);
            _interactablesTilesSetup.SetupInteractables(_gameData.CurrentLevel);
            _audioManager.StopMusic();
            _audioManager.PlayStopMusic();
            await UniTask.Delay(TimeSpan.FromSeconds(1f)); 
            await _asyncSceneLoading.UnloadAsync(Scenes.MENU);
            await _asyncSceneLoading.LoadAsync(Scenes.GAME);
            _audioManager.PlayGameMusic();
            _cts.Cancel();
        }
    }
}