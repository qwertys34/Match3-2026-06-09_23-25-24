using System;
using System.Collections.Generic;
using System.Threading;
using Animations;
using Cysharp.Threading.Tasks;
using Game.GridSystem;
using Game.MatchTiles;
using Game.Tiles;

namespace StateMachine.States
{
    public class RemoveTileState : IState, IDisposable
    {
        private IStateSwitcher _switcher;
        private Grid _grid;
        private IAnimation _animation;
        private CancellationTokenSource _cts;
        private MatchFinder _matchFinder;
        
        public RemoveTileState(IStateSwitcher switcher, Grid grid, IAnimation animation,  MatchFinder matchFinder)
        {
            _switcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
        }
        
        public async void Enter()
        {
            _cts = new CancellationTokenSource();
            // score++
            await RemoveTiles(_matchFinder.TilesToRemove);
            _switcher.SwitchState<RefillGridState>();
        }

        private async UniTask RemoveTiles(List<Tile> tilesToRemove)
        {
            foreach (var tile in tilesToRemove)
            {
                // play sound
                _grid.SetValue(tile.transform.position, null);
                await _animation.HideTile(tile.gameObject);
                // FX
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