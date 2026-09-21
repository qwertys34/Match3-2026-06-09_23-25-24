using Animations;
using Audio;
using Data;
using Game.Board;
using Game.GridSystem;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using Game.UI;
using Game.Utils;
using Levels;
using ResurcesLoading;
using Grid = Game.GridSystem.Grid;
using IInitializable = VContainer.Unity.IInitializable;
using UnityEngine;

namespace Game.EntryPoint
{
    public class GameEntryPoint : IInitializable
    {
        private BackgroundTilesSetup _backgroundTilesSetup;
        private LevelConfig _levelConfig;
        private ScoreCalculator _scoreCalculator;
        private InteractablesTilesSetup interactablesTilesSetup;
        private GameProgress _gameProgress;
        private MatchFinder _matchFinder;
        private Grid _grid;
        private GameBoard _gameBoard;
        private GameDebug _gameDebug;
        private TilePool _tilePool;
        private GameData _gameData;
        private AudioManager _audioManager;
        private IAnimation _animation;
        private GameResurcesLoader _resurcesLoader;
        private SetupCamera _setupCamera;
        private FXPool _fxPool;
        private IAsyncSceneLoading _sceneLoading;
        private StateMachine.StateMachine _stateMachine;
        private EndGamePanelView _endGamePanelView;

        private bool _isDebugging;
        
        public async void Initialize()
        {
            _levelConfig = _gameData.CurrentLevel;
            _isDebugging = true;
            if (_isDebugging)
                _gameDebug.ShowDebug(_gameBoard.transform);
            _grid.SetupGrid(_levelConfig.Width, _levelConfig.Height); 
            _gameProgress.LoadLevelConfig(_levelConfig);
            await _resurcesLoader.Load();
            
            bool isVertical = Screen.width < Screen.height;
            _setupCamera.SetCamera(_grid.Width, _grid.Height, isVertical);
            
            interactablesTilesSetup.SetupInteractables(_levelConfig);
            _stateMachine = new StateMachine.StateMachine(_gameBoard, _grid, _animation, _matchFinder, _tilePool,
                _gameProgress, _scoreCalculator, _audioManager, _endGamePanelView, _backgroundTilesSetup, _fxPool,
                _resurcesLoader);
            _sceneLoading.LoadingIsDone(true);
        }

        public GameEntryPoint(ScoreCalculator scoreCalculator, InteractablesTilesSetup interactablesTilesSetup,
            GameProgress gameProgress, MatchFinder matchFinder, Grid grid, GameBoard gameBoard, GameDebug gameDebug,
            TilePool tilePool, GameData gameData, AudioManager audioManager, IAnimation animation,
            GameResurcesLoader resurcesLoader, SetupCamera setupCamera, IAsyncSceneLoading sceneLoading,
            EndGamePanelView endGamePanelView, BackgroundTilesSetup backgroundTilesSetup, FXPool fxPool)
        {
            _scoreCalculator = scoreCalculator;
            this.interactablesTilesSetup = interactablesTilesSetup;
            _gameProgress = gameProgress;
            _matchFinder = matchFinder;
            _grid = grid;
            _gameBoard = gameBoard;
            _gameDebug = gameDebug;
            _tilePool = tilePool;
            _gameData = gameData;
            _audioManager = audioManager;
            _animation = animation;
            _resurcesLoader = resurcesLoader;
            _setupCamera = setupCamera;
            _sceneLoading = sceneLoading;
            _endGamePanelView = endGamePanelView;
            _backgroundTilesSetup = backgroundTilesSetup;
            _fxPool =  fxPool;
        }
    }
}