using System;
using System.Threading;
using Animations;
using Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Board;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using Game.Utils;
using ResurcesLoading;
using UnityEngine;
using Grid = Game.GridSystem.Grid;

namespace StateMachine.States
{
    public class MergeTilesState : IState, IDisposable
    {
        private IStateSwitcher _switcher;
        private Grid _grid;
        private IAnimation _animation;
        private CancellationTokenSource _cts;
        private MatchFinder _matchFinder;
        private GameProgress _gameProgress;
        private AudioManager _audioManager;
        private ScoreCalculator  _scoreCalculator;
        private FXPool _fxPool;
        private GameBoard _gameBoard;
        private GameResurcesLoader _resurcesLoader;
        
        public bool hasMergeTiles { get; private set; } = false;

        public MergeTilesState(IStateSwitcher switcher, Grid grid, IAnimation animation,
            MatchFinder matchFinder, GameProgress gameProgress, AudioManager audioManager,
            ScoreCalculator scoreCalculator, FXPool fxPool, GameBoard gameBoard,
            GameResurcesLoader resurcesLoader)
        {
            _switcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _gameProgress = gameProgress;
            _audioManager = audioManager;
            _scoreCalculator = scoreCalculator;
            _fxPool = fxPool;
            _gameBoard = gameBoard;
            _resurcesLoader = resurcesLoader;
        }
        

        public async void Enter()
        {
            _cts = new CancellationTokenSource();
            _audioManager.PlayWhoosh();
            await MergeTiles(_grid.CurrentPosition, _grid.TargetPosition);
            if (!hasMergeTiles)
            {
                _audioManager.PlayNoMatch();
                _switcher.SwitchState<PlayerTurnState>();
            }
            else
            {
                _gameProgress.SpendMoves();
                _switcher.SwitchState<RefillGridState>();
            }
            _cts.Cancel();
        }

        private async UniTask MergeTiles(Vector2Int current, Vector2Int target)
        {
            var superCandyTile = _grid.GetValue(target.x, target.y);
            var spTile = _grid.GetValue(current.x, current.y).GetComponent<SpriteRenderer>();
            await _animation.AnimateTile(superCandyTile, 1.2f, 0.2f);
            var matchTiles = _matchFinder.FindAnyTilesWithSprite(spTile, _grid);
            if (matchTiles.Count > 0) hasMergeTiles = true;
            
            foreach (var matchTile in matchTiles)
            {
                _ = _animation.AnimateTile(matchTile, 1.2f, 0.001f);
                // мб звук какой проиграть
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            var sizeSupCandy = 1.2f;
            foreach (var matchTile in matchTiles)
            {
                if (matchTile.tileKind == TileKind.Jelly)
                {
                    var jellyTile = (JellyTile)matchTile;
                    _ = _animation.ShakeAnimate(matchTile.transform, 0.3f, Ease.InCubic);
                    await _animation.AnimateTile(jellyTile, 1f);
                    jellyTile.ChangeState(jellyTile.JellyTransform, _resurcesLoader);
                    if (jellyTile.IsSimpleTile())
                    {
                        _scoreCalculator.CalculateAmountRemainingTiles(TileKind.Jelly);
                    }
                    if (jellyTile.CanAlive())
                        continue;
                }
                _grid.SetValue(matchTile.transform.position, null);
                _ = _animation.MoveObject(matchTile.gameObject, superCandyTile.transform.position,
                    0.3f, Ease.OutCubic);
                _audioManager.PlayMatch();
                _ = _animation.ShakeAnimate(superCandyTile.transform, 0.2f, Ease.InCubic);
                await _animation.AnimateSuperCandyMatch(superCandyTile.transform, 
                    superCandyTile.GetComponent<SpriteRenderer>(), sizeSupCandy, 0.3f); 
                sizeSupCandy = Mathf.Min(1.5f, sizeSupCandy + 0.05f);
                var amountScore = _scoreCalculator.CalculateScore(_matchFinder.CurrentMatchResult.MatchDirection);
                _fxPool.GetFX(matchTile.transform.position, _gameBoard.transform, amountScore);
                await _animation.HideTile(matchTile.gameObject);
            }
            await _animation.HideTile(superCandyTile.gameObject);
            _grid.SetValue(superCandyTile.transform.position, null);
        }
        
        public void Dispose()
        {
            _cts?.Dispose();
        }
        
        public void Exit()
        {
            _cts?.Cancel();
        }
    }
}