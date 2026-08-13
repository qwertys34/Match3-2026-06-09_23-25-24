using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Menu.Levels
{
    public class SetupLevelSequence
    {
        public LevelSequenceConfig CurrentLevelSequence { get; private set; }
        private AsyncOperationHandle<LevelSequenceConfig> _currentHandle;
        public bool isOneLevel { get; private set; }

        public async UniTask Setup(int currentLevel)
        {
            await LoadLevels("Levels1-5");
            Debug.Log("Levels1-5");
            isOneLevel = false;
            
            if (currentLevel <= 5)
            {
                await LoadLevels("Levels1-5");
                Debug.Log("Levels1-5");
                isOneLevel = false;
            }
            else if (currentLevel <= 10)
            {
                await LoadLevels("Levels6-10");
                Debug.Log("Levels6-10");
                isOneLevel = false;
            }
            else
            {
                await LoadLevels("LevelEndless");
                Debug.Log("LevelEndless");
                isOneLevel = true;
            }
        }
        
        private async UniTask LoadLevels(string key)
        {
            if (_currentHandle.IsValid())
            {
                Addressables.Release(_currentHandle);
            }

            _currentHandle = Addressables.LoadAssetAsync<LevelSequenceConfig>(key);
            await _currentHandle.ToUniTask();
            if (_currentHandle.Status == AsyncOperationStatus.Succeeded)
            {
                CurrentLevelSequence = _currentHandle.Result;
            }
        }
    }
}