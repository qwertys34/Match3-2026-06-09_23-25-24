using Menu.Levels;
using ResurcesLoading;
using VContainer.Unity;

namespace Menu
{
    public class MenuEntryPoint : IInitializable
    {
        private SetupLevelSequence _setupLevelSequence;
        private IAsyncSceneLoading _asyncSceneLoading;

        public MenuEntryPoint(SetupLevelSequence setupLevelSequence,  IAsyncSceneLoading asyncSceneLoading)
        {
            _setupLevelSequence = setupLevelSequence;
            _asyncSceneLoading = asyncSceneLoading;
        }

        public async void Initialize()
        {
            await _setupLevelSequence.Setup(8);
            // music menu
            _asyncSceneLoading.LoadingIsDone(true);
            // await animation
            // button enabled
        }
    }
}