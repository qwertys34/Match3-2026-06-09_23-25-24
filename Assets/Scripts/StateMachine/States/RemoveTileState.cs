using System;
using System.Collections.Generic;
using System.Threading;
using Animations;
using Audio;
using Cysharp.Threading.Tasks;
using Game.Board;
using Game.GridSystem;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using Game.Utils;

namespace StateMachine.States
{
    public class RemoveTileState : IState, IDisposable
    {
        private IStateSwitcher _switcher;
        private Grid _grid;
        private IAnimation _animation;
        private CancellationTokenSource _cts;
        private MatchFinder _matchFinder;
        private ScoreCalculator _scoreCalculator;
        private AudioManager _audioManager;
        private FXPool _fxPool;
        private GameBoard _gameBoard;
        
        public RemoveTileState(IStateSwitcher switcher, Grid grid, IAnimation animation,  MatchFinder matchFinder,
            ScoreCalculator scoreCalculator,  AudioManager audioManager, FXPool fxPool, GameBoard gameBoard)
        {
            _switcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _scoreCalculator = scoreCalculator;
            _audioManager = audioManager;
            _fxPool = fxPool;
            _gameBoard = gameBoard;
        }
        
        public async void Enter()
        {
            _cts = new CancellationTokenSource();
            _scoreCalculator.CalculateScoreToAdd(_matchFinder.CurrentMatchResult.MatchDirection);
            await RemoveTiles(_matchFinder.TilesToRemove);
            _switcher.SwitchState<RefillGridState>();
        }

        private async UniTask RemoveTiles(List<Tile> tilesToRemove)
        {
            foreach (var tile in tilesToRemove)
            {
                _audioManager.PlayRemove();
                _grid.SetValue(tile.transform.position, null);
                await _animation.HideTile(tile.gameObject);
                _fxPool.GetFX(tile.transform.position, _gameBoard.transform);
            }
            _cts.Cancel();
        }
        
        public void Exit()
        {
            _matchFinder.ClearTilesToRemove();
            _cts?.Cancel();
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}