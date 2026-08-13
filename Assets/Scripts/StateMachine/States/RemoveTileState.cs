using System;
using System.Collections.Generic;
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
using Grid = Game.GridSystem.Grid;

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
            await RemoveAnyTiles(_matchFinder.TilesToRemove, _matchFinder.BlankTilesToRemove, _matchFinder.VerticalRocketTilesToRemove,
                _matchFinder.HorizontalRocketTilesToRemove);
            _switcher.SwitchState<RefillGridState>();
        }

        private async UniTask RemoveAnyTiles(List<Tile> tilesToRemove, List<BlankTile> blankTilesToRemove,
            List<Tile> verticalRocketTilesToRemove, List<Tile> horizontalRocketTilesToRemove)
        {
            foreach (var rocketTile in verticalRocketTilesToRemove)
            {
                await ((VerticalRocketTile)rocketTile).Run(_grid, _animation, _scoreCalculator, _fxPool, _matchFinder,
                    _gameBoard, _audioManager, _gameResurcesLoader, horizontalRocketTilesToRemove);
            }
            
            foreach (var rocketTile in horizontalRocketTilesToRemove)
            {
                await ((HorizontalRocketTile)rocketTile).Run(_grid, _animation, _scoreCalculator, _fxPool, _matchFinder,
                    _gameBoard, _audioManager, _gameResurcesLoader, verticalRocketTilesToRemove);
            }
            
            foreach (var tile in tilesToRemove)
            {
                if (tile.tileKind == TileKind.Jelly) // тут важно именно поле проверять
                {
                    var  jellyTile = (JellyTile)tile;
                    jellyTile.ChangeState(jellyTile.JellyTransform, _gameResurcesLoader);
                    if (jellyTile.CanAlive())
                    {
                        _audioManager.PlayPop();
                        await _animation.ShakeAnimate(tile.transform, 0.1f, Ease.InQuint);
                        var amScore = _scoreCalculator.CalculateScore(_matchFinder.CurrentMatchResult.MatchDirection);
                        _fxPool.GetFX(tile.transform.position, _gameBoard.transform, amScore);
                        // fx скорее всего не нужен, если я не буду конечно давать очков за ломку jelly
                        if (jellyTile.IsSimpleTile()) // если jelly на тайле уже нет
                            _scoreCalculator.CalculateAmountRemainingTiles(TileKind.Jelly); 
                        continue;
                    }
                }
                else if (tile.tileKind == TileKind.Bomb)
                {
                    await ((BombTile)tile).Explode(_grid, _gameResurcesLoader, _animation, _scoreCalculator,
                        _fxPool, _gameBoard, _audioManager, _matchFinder);
                    continue;
                }
                _audioManager.PlayRemove();
                _grid.SetValue(tile.transform.position, null);
                await _animation.HideTile(tile.gameObject);
                var amountScore = _scoreCalculator.CalculateScore(_matchFinder.CurrentMatchResult.MatchDirection);
                _fxPool.GetFX(tile.transform.position, _gameBoard.transform, amountScore);
                // fadsfl;ds
            }
            foreach (var blankTile in blankTilesToRemove)
            {
                await _animation.ShakeAnimate(blankTile.transform, 0.1f, Ease.InQuint);
                blankTile.ChangeState(_gameResurcesLoader);
                if (blankTile.CanAlive()) continue;
                
                _scoreCalculator.CalculateAmountRemainingTiles(TileKind.Blank);
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