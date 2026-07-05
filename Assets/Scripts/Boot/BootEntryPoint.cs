using System;
using DG.Tweening;
using SceneLoading;
using UnityEngine;
using VContainer.Unity;

namespace Boot
{
    public class BootEntryPoint : IInitializable
    {
        private IAsyncSceneLoading _sceneLoading;

        public BootEntryPoint(IAsyncSceneLoading sceneLoading) => _sceneLoading = sceneLoading;

        public async void Initialize()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            DOTween.SetTweensCapacity(5000, 100);
            await _sceneLoading.LoadAsync(Scenes.MENU);
            try
            {
                
            }
            catch (OperationCanceledException)
            { }
        }
    }
}