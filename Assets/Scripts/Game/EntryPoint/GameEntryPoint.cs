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

namespace Game.EntryPoint
{
    public class GameEntryPoint : IInitializable
    {
        // BG Tile Setup
        private LevelConfig _levelConfig;
        private ScoreCalculator _scoreCalculator;
        private BlankTilesSetup _blankTilesSetup;
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
        // FX pool
        private IAsyncSceneLoading _sceneLoading;
        private StateMachine.StateMachine _stateMachine;
        private EndGamePanelView _endGamePanelView;

        private bool _isDebugging;
        
        public void Initialize()
        {
            _levelConfig = _gameData.CurrentLevel;
            if (_isDebugging)
                _gameDebug.ShowDebug(_gameBoard.transform);
            _grid.SetupGrid(_levelConfig.Width, _levelConfig.Height);
            _gameProgress.LoadLevelConfig(_levelConfig.GoalScore, _levelConfig.Moves);
            // await resurces 
            _setupCamera.SetCamera(_grid.Width, _grid.Height, false);
            _blankTilesSetup.SetupBlanks(_levelConfig);
            _stateMachine = new StateMachine.StateMachine(_gameBoard, _grid, _animation, _matchFinder, _tilePool,
                _gameProgress, _scoreCalculator, _audioManager, _endGamePanelView);
            _sceneLoading.LoadingIsDone(true);
        }

        public GameEntryPoint(ScoreCalculator scoreCalculator, BlankTilesSetup blankTilesSetup,
            GameProgress gameProgress, MatchFinder matchFinder, Grid grid, GameBoard gameBoard, GameDebug gameDebug,
            TilePool tilePool, GameData gameData, AudioManager audioManager, IAnimation animation,
            GameResurcesLoader resurcesLoader, SetupCamera setupCamera, IAsyncSceneLoading sceneLoading,
            EndGamePanelView endGamePanelView)
        {
            _scoreCalculator = scoreCalculator;
            _blankTilesSetup = blankTilesSetup;
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
        }
    }
}