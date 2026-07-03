using System;
using System.Threading;
using Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using UnityEngine;
using Grid = Game.GridSystem.Grid;

namespace StateMachine.States
{
    public class SwapTilesState : IState, IDisposable
    {
        private IStateSwitcher _switcher;
        private Grid _grid;
        private IAnimation _animation;
        private CancellationTokenSource _cts;
        private MatchFinder _matchFinder;
        private GameProgress _gameProgress;

        public SwapTilesState(IStateSwitcher switcher, Grid grid, IAnimation animation,
            MatchFinder matchFinder, GameProgress gameProgress)
        {
            _switcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _gameProgress = gameProgress;
        }
        

        public async void Enter()
        {
            _cts = new CancellationTokenSource();
            // play sound
            await SwapTiles(_grid.CurrentPosition, _grid.TargetPosition);
            if (_matchFinder.CheckBoardForMatches(_grid) == false)
            {
                // play no match
                await SwapTiles(_grid.TargetPosition, _grid.CurrentPosition);
                _switcher.SwitchState<PlayerTurnState>();
            }
            else
            {
                // play sound match
                // spend move
                _gameProgress.SpendMoves();
                _switcher.SwitchState<RemoveTileState>();
            }
        }

        private async UniTask SwapTiles(Vector2Int current, Vector2Int target)
        {
            var currentTile = _grid.GetValue(current.x, current.y);
            var targetTile = _grid.GetValue(target.x, target.y);
            
            AnimateTile(currentTile, target);
            AnimateTile(targetTile, current);
            
            _grid.SetValue(target.x, target.y, currentTile);
            _grid.SetValue(current.x, current.y, targetTile);
            
            _animation.AnimateTile(currentTile, 1f);
            _animation.AnimateTile(targetTile, 1f);
            
            await UniTask.WaitForSeconds(0.2f, _cts.IsCancellationRequested);
        }

        private void AnimateTile(Tile tile, Vector2Int position)
        {
            _animation.MoveTile(tile, _grid.GridToWorld(position.x, position.y), Ease.OutCubic);
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