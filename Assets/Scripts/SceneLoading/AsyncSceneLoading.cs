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
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SceneLoading
{
    public class AsyncSceneLoading : IAsyncSceneLoading, IDisposable
    {
        private class LoadedSceneInfo
        {
            public SceneInstance SceneInstance;
            public AsyncOperationHandle<SceneInstance> Handle;
        }
        
        private Dictionary<string, LoadedSceneInfo> _loadedScenes = new();
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
            try
            {
                LoadingIsDone(false);
                await UniTask.Delay(TimeSpan.FromSeconds(2));
                
                var handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                var loadedScene = await handle;
                
                if (loadedScene.Scene.IsValid())
                {
                    SceneManager.SetActiveScene(loadedScene.Scene);
                    
                    // Сохраняем и сцену и handle
                    _loadedScenes[sceneName] = new LoadedSceneInfo
                    {
                        SceneInstance = loadedScene,
                        Handle = handle
                    };
                    
                    _audioManager.PlayGameMusic();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load scene {sceneName}: {ex.Message}");
            }
            finally
            {
                LoadingIsDone(true);
            }
        }

        public async UniTask UnloadAsync(string sceneName)
        {
            if (!_loadedScenes.ContainsKey(sceneName))
            {
                Debug.LogWarning($"Scene {sceneName} is not loaded");
                return;
            }
            
            var sceneInfo = _loadedScenes[sceneName];
            
            try
            {
                if (sceneInfo.Handle.IsValid())
                {
                    await Addressables.UnloadSceneAsync(sceneInfo.Handle);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to unload scene {sceneName}: {ex.Message}");
            }
            finally
            {
                _loadedScenes.Remove(sceneName);
            }
        }

        public async UniTask RestartScene()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            await UnloadAsync(sceneName);
            await LoadAsync(sceneName);
        }

        private async UniTask LoadNextSceneByLevel(LevelConfig level, AudioManager audioManager)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            audioManager.StopAllSounds();
            audioManager.PlayStopMusic();
            _gameData.SetCurrentLevel(level);
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f)); 
            await UnloadAsync(sceneName);
            await LoadAsync(sceneName);
            
            audioManager.PlayGameMusic();
        }

        public bool IsLastLevel(SetupLevelSequence _setupLevelSequence)
        {
            var length = _setupLevelSequence.CurrentLevelSequence.LevelConfigs.Count;
            int lastNumber = _setupLevelSequence.CurrentLevelSequence.LevelConfigs[length - 1].LevelNumber;
            if (_gameData.CurrentLevelIndex >= lastNumber && lastNumber == 10)
                return true;
            return false;
        }
        
        public async UniTask LoadNewNextScene(AudioManager audioManager, SetupLevelSequence _setupLevelSequence)
        {
            _gameData.OpenNextLevel();
            _saveProgress.Save();
            int number = _gameData.CurrentLevelIndex;
            var length = _setupLevelSequence.CurrentLevelSequence.LevelConfigs.Count;
            Debug.Log("last level number: "+_setupLevelSequence.CurrentLevelSequence.LevelConfigs[length-1].LevelNumber);
    
            if (_gameData.CurrentLevelIndex <= 5)
            {
                await LoadNextSceneByLevel(
                    _setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 1], 
                    audioManager);
            }
            else
            {
                await _setupLevelSequence.Setup(_gameData.CurrentLevelIndex);
                await LoadNextSceneByLevel(
                    _setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 6],
                    audioManager);
            }
        }
        public async UniTask LoadNextScene(AudioManager audioManager, SetupLevelSequence _setupLevelSequence)
        {
            int number = _gameData.CurrentLevel.LevelNumber + 1;
            var length = _setupLevelSequence.CurrentLevelSequence.LevelConfigs.Count;
            Debug.Log("last level number: "+_setupLevelSequence.CurrentLevelSequence.LevelConfigs[length-1].LevelNumber);
    
            if (_gameData.CurrentLevelIndex <= 5)
            {
                await LoadNextSceneByLevel(
                    _setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 1], 
                    audioManager);
            }
            else
            {
                await _setupLevelSequence.Setup(_gameData.CurrentLevelIndex);
                await LoadNextSceneByLevel(
                    _setupLevelSequence.CurrentLevelSequence.LevelConfigs[number - 6],
                    audioManager);
            }
        }

        public void LoadingIsDone(bool value) => loadingScreen.SetActiveScreen(!value);

        public void Dispose()
        {
            // Отменяем все текущие операции
            cts?.Cancel();
            
            // Выгружаем все загруженные сцены
            foreach (var sceneName in _loadedScenes.Keys)
            {
                var sceneInfo = _loadedScenes[sceneName];
                if (sceneInfo.Handle.IsValid())
                {
                    try
                    {
                        // Синхронная выгрузка при Dispose
                        Addressables.UnloadSceneAsync(sceneInfo.Handle).WaitForCompletion();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to unload scene {sceneName} during dispose: {ex.Message}");
                    }
                }
            }
            
            _loadedScenes.Clear();
            cts?.Dispose();
        }
    }
}