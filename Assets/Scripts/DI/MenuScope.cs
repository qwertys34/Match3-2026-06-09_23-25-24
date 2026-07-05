using Menu;
using Menu.Levels;
using SceneLoading;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class MenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MenuEntryPoint>();
            builder.Register<SetupLevelSequence>(Lifetime.Singleton);
        }
    }
}