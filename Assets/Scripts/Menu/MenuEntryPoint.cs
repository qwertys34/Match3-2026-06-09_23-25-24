using System.Reflection;
using Audio;
using Data;
using Menu.Levels;
using Menu.UI;
using UnityEngine;
using VContainer.Unity;

namespace Menu
{
    public class MenuEntryPoint : IInitializable
    {
        private SetupLevelSequence _setupLevelSequence;
        private IAsyncSceneLoading _asyncSceneLoading;
        private LevelSequenceView _levelSequenceView;
        private MenuView _menuView;
        private AudioManager _audioManager;
        private GameData _gameData;
        
        public MenuEntryPoint(SetupLevelSequence setupLevelSequence,  IAsyncSceneLoading asyncSceneLoading,
            LevelSequenceView levelSequenceView,  MenuView menuView,  AudioManager audioManager, GameData gameData)
        {
            _setupLevelSequence = setupLevelSequence;
            _asyncSceneLoading = asyncSceneLoading;
            _levelSequenceView = levelSequenceView;
            _menuView = menuView;
            _audioManager = audioManager;
            _gameData = gameData;
        }

        public async void Initialize()
        {
            Debug.Log(_gameData.CurrentLevelIndex);
            /*if (_gameData.CurrentLevelIndex >= _setupLevelSequence.CurrentLevelSequence.LevelConfigs.Count)
                await _setupLevelSequence.Setup(_gameData.CurrentLevelIndex);*/
            await _setupLevelSequence.Setup(_gameData.CurrentLevelIndex);
            _levelSequenceView.SetupButtonsView(_gameData.CurrentLevelIndex, _setupLevelSequence.isOneLevel);
            _audioManager.PlayMenuMusic();
            _asyncSceneLoading.LoadingIsDone(true);
            await _menuView.StartAnimation();
        }
    }
}