using System;
using Animations;
using Audio;
using Boot;
using Data;
using Save;
using SceneLoading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private LoadingView loadingView;
        [SerializeField] private AudioManager audioManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BootEntryPoint>();
            builder.Register<IAsyncSceneLoading, AsyncSceneLoading>(Lifetime.Singleton);
            builder.Register<IAnimation, AnimationManager>(Lifetime.Singleton);
            builder.Register<GameData>(Lifetime.Singleton);
            builder.Register<SaveProgress>(Lifetime.Singleton);
            builder.RegisterInstance(loadingView);
            builder.RegisterInstance(audioManager);
        }
    }
}