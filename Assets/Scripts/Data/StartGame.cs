using System;
using System.Threading;
using Audio;
using Cysharp.Threading.Tasks;
using Levels;
using SceneLoading;

namespace Data
{
    public class StartGame
    {
        private GameData _gameData;
        private AudioManager _audioManager;
        private IAsyncSceneLoading _asyncSceneLoading;
        private CancellationTokenSource _cts;
        
        public StartGame(GameData gameData, AudioManager audioManager, IAsyncSceneLoading asyncSceneLoading)
        {
            _gameData = gameData;
            _audioManager = audioManager;
            _asyncSceneLoading = asyncSceneLoading;
        }

        public async UniTask Start(LevelConfig level)
        {
            _cts = new CancellationTokenSource();
            _gameData.SetCurrentLevel(level);
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