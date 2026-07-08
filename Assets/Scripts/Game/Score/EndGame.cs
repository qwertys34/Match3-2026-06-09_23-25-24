using Audio;
using Data;
using SceneLoading;

namespace Game.Score
{
    public class EndGame
    {
        private AudioManager _audioManager;
        private IAsyncSceneLoading sceneLoading;
        private GameData _gameData;

        public EndGame(AudioManager audioManager, IAsyncSceneLoading sceneLoading, GameData gameData)
        {
            _audioManager = audioManager;
            this.sceneLoading = sceneLoading;
            _gameData = gameData;
        }

        public async void End(bool success)
        {
            if (success && _gameData.CurrentLevel.LevelNumber == _gameData.CurrentLevelIndex) 
                _gameData.OpenNextLevel();
            _audioManager.StopMusic();
            await sceneLoading.UnloadAsync(Scenes.GAME);
            await sceneLoading.LoadAsync(Scenes.MENU);
            _audioManager.PlayMenuMusic();
        }
    }
}