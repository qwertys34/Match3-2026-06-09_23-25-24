using Audio;
using Cysharp.Threading.Tasks;
using Menu.Levels;

public interface IAsyncSceneLoading
{
    UniTask LoadAsync(string  sceneName);
    UniTask RestartScene();
    UniTask UnloadAsync(string  sceneName);
    UniTask LoadNextScene(AudioManager audioManager, SetupLevelSequence _setupLevelSequence);
    bool IsLastLevel(SetupLevelSequence _setupLevelSequence);
    void LoadingIsDone(bool value);
}