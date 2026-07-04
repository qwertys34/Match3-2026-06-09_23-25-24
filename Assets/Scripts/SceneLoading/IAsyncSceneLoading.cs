using Cysharp.Threading.Tasks;


public interface IAsyncSceneLoading
{
    UniTask LoadAsync(string  sceneName);
    UniTask UnloadAsync(string  sceneName);
    void LoadingIsDone(bool value);
}