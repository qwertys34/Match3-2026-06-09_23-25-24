using Animations;
using Data;
using Menu;
using Menu.Levels;
using Menu.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class MenuScope : LifetimeScope
    {
        [SerializeField] private LevelSequenceView levelSequenceView; 
        [SerializeField] private MenuView menuView; 
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MenuEntryPoint>();
            builder.Register<SetupLevelSequence>(Lifetime.Singleton);
            builder.Register<StartGame>(Lifetime.Singleton);
            builder.RegisterInstance(levelSequenceView);
            builder.RegisterInstance(menuView);
            builder.Register<AnimationManager>(Lifetime.Singleton);
        }
    }
}