using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SceneLoading
{
    public class AsyncSceneLoading : IAsyncSceneLoading
    {
        private Dictionary<string, SceneInstance> _loadedScenes = new();
        private LoadingView loadingScreen;
        private CancellationTokenSource cts;
        
        public AsyncSceneLoading(LoadingView loadingScreen)
        {
            this.loadingScreen = loadingScreen;
        }

        public async UniTask LoadAsync(string sceneName)
        {
            cts = new CancellationTokenSource();
            LoadingIsDone(false);
            await UniTask.Delay(2000);
            var loadedScene = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                .WithCancellation(cts.Token);
            SceneManager.SetActiveScene(loadedScene.Scene);
            _loadedScenes.TryAdd(sceneName, loadedScene); // если нет - добавляем
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

        public void LoadingIsDone(bool value) => loadingScreen.SetActiveScreen(!value);
    }
}