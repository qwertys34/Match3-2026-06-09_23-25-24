using System.Collections.Generic;
using System.Linq;
using Animations;
using Audio;
using Game.Board;
using Game.GridSystem;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using Game.UI;
using Game.Utils;
using ResurcesLoading;
using StateMachine.States;

namespace StateMachine
{
    public class StateMachine : IStateSwitcher
    {
        private List<IState> _states;
        private IState _currentState;
        private GameBoard _gameBoard;
        private Grid _grid;
        private IAnimation _animation;
        private MatchFinder _matchFinder;
        private TilePool _tilePool;
        private GameProgress _gameProgress;
        private ScoreCalculator _scoreCalculator;
        private AudioManager _audioManager;
        private EndGamePanelView _endGamePanelView;
        private BackgroundTilesSetup _backgroundTilesSetup;
        private FXPool _fxPool;
        private GameResurcesLoader _gameResurcesLoader;
        
        public StateMachine(GameBoard gameBoard,  Grid grid,  IAnimation animation, MatchFinder matchFinder,
            TilePool tilePool,  GameProgress gameProgress,  ScoreCalculator scoreCalculator,
            AudioManager audioManager, EndGamePanelView endGamePanelView, BackgroundTilesSetup
                backgroundTilesSetup, FXPool fxPool,  GameResurcesLoader gameResurcesLoader)
        {
            _gameBoard = gameBoard;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _tilePool = tilePool;
            _gameProgress = gameProgress;
            _scoreCalculator = scoreCalculator;
            _audioManager = audioManager;
            _endGamePanelView = endGamePanelView;
            _backgroundTilesSetup = backgroundTilesSetup;
            _fxPool = fxPool;
            _gameResurcesLoader = gameResurcesLoader;
            _states = new List<IState>() {
                new PrepareState(this, _gameBoard, _backgroundTilesSetup, _grid),
                new PlayerTurnState(this, _animation, _grid, _audioManager), 
                new SwapTilesState(this, _grid, _animation, _matchFinder, _gameProgress, _audioManager),
                new RemoveTileState(this, _grid, _animation, _matchFinder, _scoreCalculator, _audioManager,
                    _fxPool, _gameBoard, _gameResurcesLoader),
                new RefillGridState(this, _grid, _animation, _matchFinder, _tilePool,
                    _gameBoard.transform, _gameProgress, _audioManager),
                new WinState(_endGamePanelView),
                new LooseState(_endGamePanelView)
            };
            
            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : IState
        {
            var newState = _states.FirstOrDefault(state => state is T);
            _currentState.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }
    }
}