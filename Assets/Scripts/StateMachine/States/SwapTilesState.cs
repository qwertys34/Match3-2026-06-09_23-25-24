using System;
using System.Threading;
using Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        public SwapTilesState(IStateSwitcher switcher, Grid grid, IAnimation animation)
        {
            _switcher = switcher;
            _grid = grid;
            _animation = animation;
        }
        

        public async void Enter()
        {
            _cts = new CancellationTokenSource();
            // play sound
            await SwapTiles(_grid.CurrentPosition, _grid.TargetPosition);
            _switcher.SwitchState<PlayerTurnState>();
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