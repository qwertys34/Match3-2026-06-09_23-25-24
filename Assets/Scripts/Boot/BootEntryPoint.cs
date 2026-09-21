using DG.Tweening;
using Save;
using SceneLoading;
using UnityEngine;
using VContainer.Unity;
using YG;

namespace Boot
{
    public class BootEntryPoint : IInitializable
    {
        private IAsyncSceneLoading _sceneLoading;
        private SaveProgress _saveProgress;

        public BootEntryPoint(IAsyncSceneLoading sceneLoading,  SaveProgress saveProgress)
        {
            _sceneLoading = sceneLoading;
            _saveProgress = saveProgress;
        }


        public async void Initialize()
        {
            YG2.StickyAdActivity(true);
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //_saveProgress.ResetProgress(); 
            _saveProgress.Load();
            DOTween.SetTweensCapacity(5000, 100);
            await _sceneLoading.LoadAsync(Scenes.MENU);
        }
    }
}