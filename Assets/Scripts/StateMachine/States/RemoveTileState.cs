using System;
using System.Collections.Generic;
using System.Threading;
using Animations;
using Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Board;
using Game.GridSystem;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using Game.Utils;
using ResurcesLoading;

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
        private GameResurcesLoader  _gameResurcesLoader;
        
        public RemoveTileState(IStateSwitcher switcher, Grid grid, IAnimation animation,  MatchFinder matchFinder,
            ScoreCalculator scoreCalculator,  AudioManager audioManager, FXPool fxPool, GameBoard gameBoard,
            GameResurcesLoader gameResurcesLoader)
        {
            _switcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _scoreCalculator = scoreCalculator;
            _audioManager = audioManager;
            _fxPool = fxPool;
            _gameBoard = gameBoard;
            _gameResurcesLoader = gameResurcesLoader;
        }
        
        public async void Enter()
        {
            _cts = new CancellationTokenSource();
            _scoreCalculator.CalculateScoreToAdd(_matchFinder.CurrentMatchResult.MatchDirection);
            await RemoveAnyTiles(_matchFinder.TilesToRemove, _matchFinder.BlankTilesToRemove);
            _switcher.SwitchState<RefillGridState>();
        }

        private async UniTask RemoveAnyTiles(List<Tile> tilesToRemove, List<BlankTile> blankTilesToRemove)
        {
            foreach (var tile in tilesToRemove)
            {
                _audioManager.PlayRemove();
                _grid.SetValue(tile.transform.position, null);
                await _animation.HideTile(tile.gameObject);
                var amountScore = _scoreCalculator.CalculateScore(_matchFinder.CurrentMatchResult.MatchDirection);
                _fxPool.GetFX(tile.transform.position, _gameBoard.transform, amountScore);
                // fadsfl;ds
            }
            foreach (var blankTile in blankTilesToRemove)
            {
                await _animation.ShakeAnimate(blankTile.transform, 0.1f, Ease.InQuint);//111
                blankTile.ChangeState(_gameResurcesLoader);
                if (blankTile.CanAlive()) continue;
                
                _audioManager.PlayRemove();
                _grid.SetValue(blankTile.transform.position, null);
                await _animation.HideTile(blankTile.gameObject);
                // fadsfl;ds
            }
            _cts.Cancel();
        }
        
        public void Exit()
        {
            _matchFinder.ClearAnyTilesToRemove(); 
            _cts?.Cancel();
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}