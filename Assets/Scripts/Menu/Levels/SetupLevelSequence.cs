using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Menu.Levels
{
    public class SetupLevelSequence : IDisposable
    {
        public LevelSequenceConfig CurrentLevelSequence { get; private set; }
        private AsyncOperationHandle<LevelSequenceConfig> _currentHandle;
        public bool isOneLevel { get; private set; }

        public async UniTask Setup(int currentLevel)
        {
            // Определяем какой ключ загружать
            string keyToLoad;
            
            if (currentLevel <= 5)
            {
                keyToLoad = "Levels1-5";
                isOneLevel = false;
            }
            else if (currentLevel <= 10)
            {
                keyToLoad = "Levels6-10";
                isOneLevel = false;
            }
            else
            {
                keyToLoad = "LevelEndless";
                isOneLevel = true;
            }
            
            Debug.Log($"Loading levels: {keyToLoad}");
            await LoadLevels(keyToLoad);
        }
        
        private async UniTask LoadLevels(string key)
        {
            // Освобождаем предыдущий handle если он валиден
            if (_currentHandle.IsValid())
            {
                try
                {
                    Addressables.Release(_currentHandle);
                    Debug.Log($"Released previous handle for: {key}");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to release previous handle: {ex.Message}");
                }
            }

            // Загружаем новый
            _currentHandle = Addressables.LoadAssetAsync<LevelSequenceConfig>(key);
            
            try
            {
                await _currentHandle.ToUniTask();
                
                if (_currentHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    CurrentLevelSequence = _currentHandle.Result;
                    Debug.Log($"Successfully loaded: {key}");
                }
                else
                {
                    Debug.LogError($"Failed to load {key}. Status: {_currentHandle.Status}");
                    // Освобождаем неудачный handle
                    if (_currentHandle.IsValid())
                    {
                        Addressables.Release(_currentHandle);
                    }
                    _currentHandle = default;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception loading {key}: {ex.Message}");
                if (_currentHandle.IsValid())
                {
                    Addressables.Release(_currentHandle);
                }
                _currentHandle = default;
            }
        }
        
        public void Dispose()
        {
            if (_currentHandle.IsValid())
            {
                try
                {
                    Debug.Log($"SetupLevelSequence.Dispose: Releasing handle");
                    Addressables.Release(_currentHandle);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to release handle in Dispose: {ex.Message}");
                }
                finally
                {
                    _currentHandle = default;
                }
            }
        }
    }
}