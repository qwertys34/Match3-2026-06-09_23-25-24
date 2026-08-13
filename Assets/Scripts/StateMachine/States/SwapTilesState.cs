using System;
using System.Threading;
using Animations;
using Audio;
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
        private AudioManager _audioManager;

        public SwapTilesState(IStateSwitcher switcher, Grid grid, IAnimation animation,
            MatchFinder matchFinder, GameProgress gameProgress, AudioManager audioManager)
        {
            _switcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _gameProgress = gameProgress;
            _audioManager = audioManager;
        }
        

        public async void Enter()
        {
            try
            {
                _cts = new CancellationTokenSource();
                _audioManager.PlayWhoosh();
                await SwapTiles(_grid.CurrentPosition, _grid.TargetPosition);
                if (_matchFinder.CheckBoardForMatches(_grid) == false)
                {
                    _audioManager.PlayNoMatch();
                    await SwapTiles(_grid.TargetPosition, _grid.CurrentPosition);
                    _switcher.SwitchState<PlayerTurnState>();
                }
                else
                {
                    _matchFinder.CheckToBlankTiles(_grid); 
                    _matchFinder.CheckOnRocketTiles(_grid);
                    _audioManager.PlayMatch();
                    _gameProgress.SpendMoves();
                    _switcher.SwitchState<RemoveTileState>();
                }
                _cts.Cancel();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async UniTask SwapTiles(Vector2Int current, Vector2Int target)
        {
            var currentTile = _grid.GetValue(current.x, current.y);
            var targetTile = _grid.GetValue(target.x, target.y);
            
            AnimateTile(currentTile, target, true, out var tileSrOne, out var modefireSrOne);
            AnimateTile(targetTile, current, false, out var tileSrTwo, out var modefireSrTwo);
            
            _grid.SetValue(target.x, target.y, currentTile);
            _grid.SetValue(current.x, current.y, targetTile);
            
            _ = _animation.AnimateTile(currentTile, 1f);
            _ = _animation.AnimateTile(targetTile, 1f);
            
            await UniTask.WaitForSeconds(0.2f, _cts.IsCancellationRequested);
            
            tileSrOne.sortingOrder = 0;
            modefireSrOne.sortingOrder = 0;
        }
        
        private void AnimateTile(Tile tile, Vector2Int position, bool isFirst, out SpriteRenderer tileSR,
            out SpriteRenderer modefireSR)
        {
            tileSR = tile.GetComponentInParent<SpriteRenderer>();
            tileSR.sortingOrder = isFirst ? 1 : 0;
            modefireSR = tile.gameObject.GetComponentInChildren<SpriteRenderer>();
            modefireSR.sortingOrder = isFirst ? 1 : 0;
            _animation.MoveTile(tile,_grid.GridToWorld(position.x, position.y),Ease.OutCubic);
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