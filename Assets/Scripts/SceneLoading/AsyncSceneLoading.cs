using System;
using System.Collections.Generic;
using System.Threading;
using Audio;
using Cysharp.Threading.Tasks;
using Data;
using Levels;
using Menu.Levels;
using Save;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SceneLoading
{
    public class AsyncSceneLoading : IAsyncSceneLoading, IDisposable
    {
        private Dictionary<string, SceneInstance> _loadedScenes = new();
        private LoadingView loadingScreen;
        private CancellationTokenSource cts;
        private GameData _gameData;
        private SaveProgress _saveProgress;
        private AudioManager _audioManager;
        
        public AsyncSceneLoading(LoadingView loadingScreen, GameData gameData, SaveProgress saveProgress,
            AudioManager audioManager)
        {
            this.loadingScreen = loadingScreen;
            _gameData = gameData;
            _saveProgress = saveProgress;
            _audioManager = audioManager;
        }

        public async UniTask LoadAsync(string sceneName)
        {
            cts = new CancellationTokenSource();
            LoadingIsDone(false);
            await UniTask.Delay(TimeSpan.FromSeconds(2), cts.IsCancellationRequested);
           var loadedScene = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                .WithCancellation(cts.Token);
            SceneManager.SetActiveScene(loadedScene.Scene);
            _loadedScenes.TryAdd(sceneName, loadedScene);
            _audioManager.PlayGameMusic();
            cts.Cancel();
        }

        public async UniTask UnloadAsync(string sceneName)
        {
            cts = new CancellationTokenSource();
            var scene = _loadedScenes[sceneName];
            await Addressables.UnloadSceneAsync(scene).WithCancellation(cts.Token).AsUniTask();
            _loadedScenes.Remove(sceneName);    
            cts.Cancel(); 
        }

        public async UniTask RestartScene()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            await UnloadAsync(sceneName);
            await LoadAsync(sceneName);
        }

        private async UniTask LoadNextSceneByLevel(LevelConfig level, AudioManager audioManager)
        {
            cts = new CancellationTokenSource();
            var sceneName = SceneManager.GetActiveScene().name;
            audioManager.StopAllSounds(); //audioManager.StopMusic();
            audioManager.PlayStopMusic();
            _gameData.SetCurrentLevel(level);
            await UniTask.Delay(TimeSpan.FromSeconds(1f)); 
            await UnloadAsync(sceneName);
            await LoadAsync(sceneName);
            audioManager.PlayGameMusic();
            cts.Cancel();
        }

        public bool IsLastLevel(SetupLevelSequence _setupLevelSequence)
        {
            var length = _setupLevelSequence.CurrentLevelSequence.LevelConfigs.Count; // всегда 5 будет
            int lastNumber = _setupLevelSequence.CurrentLevelSequence.LevelConfigs[length - 1].LevelNumber;
            if (_gameData.CurrentLevelIndex >= lastNumber && lastNumber == 10)
                return true;
            return false;//
        }
        
        public async UniTask LoadNextScene(AudioManager audioManager, SetupLevelSequence _setupLevelSequence)
        {
            _gameData.OpenNextLevel();
            //await UniTask.Delay(TimeSpan.FromSeconds(1f));
            _saveProgress.Save();
            int number = _gameData.CurrentLevelIndex;
            var length = _setupLevelSequence.CurrentLevelSequence.LevelConfigs.Count; // всегда 5 будет
            Debug.Log("last level number: "+_setupLevelSequence.CurrentLevelSequence.LevelConfigs[length-1].LevelNumber);
    
            
            if (_gameData.CurrentLevelIndex <= 5)
            {
                await LoadNextSceneByLevel(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 1], audioManager);
            }
            else
            {
                await _setupLevelSequence.Setup(_gameData.CurrentLevelIndex);
                await LoadNextSceneByLevel(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 6],
                    audioManager);
            }   
            
            /*if (_setupLevelSequence.CurrentLevelSequence.LevelConfigs[length-1].LevelNumber == 5)
            {
                await LoadNextSceneByLevel(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 1], audioManager);
            }
            else
                await LoadNextSceneByLevel(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 6],
                    audioManager);*/

        }

        public void LoadingIsDone(bool value) => loadingScreen.SetActiveScreen(!value);

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}