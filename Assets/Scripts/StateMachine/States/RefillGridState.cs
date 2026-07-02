using System;
using System.Collections.Generic;
using System.Threading;
using Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.MatchTiles;
using Game.Tiles;
using UnityEngine;
using Grid = Game.GridSystem.Grid;

namespace StateMachine.States
{
    public class RefillGridState : IState, IDisposable
    {
        private IStateSwitcher _stateSwitcher;
        private Grid _grid;
        private IAnimation _animation;
        private CancellationTokenSource _cts;
        private MatchFinder _matchFinder;
        private TilePool _tilePool;
        private Transform _parent;

        private List<Vector2Int> _tilesToRefillPos = new List<Vector2Int>(); 
        
        public RefillGridState(IStateSwitcher switcher, Grid grid, IAnimation animation,
            MatchFinder matchFinder, TilePool tilePool, Transform parent)
        {
            _stateSwitcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _tilePool = tilePool;
            _parent = parent;
        }

        public void Dispose() => _cts?.Dispose();

        public async void Enter()
        {
            await TileFall();
            await CreateEmptyTiles();
            if (_matchFinder.CheckBoardForMatches(_grid))
            {
                _stateSwitcher.SwitchState<RemoveTileState>();
                // play sound
            }
            else
            {
                CheckEndGame();
                // play sound
            }
        }

        private async UniTask TileFall()
        {
            _cts = new CancellationTokenSource();
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (_grid.GetValue(x, y)) continue;
                    for (int h = y+1; h < _grid.Height; h++)
                    {
                        if (_grid.GetValue(x, h) == null) continue;
                        if (_grid.GetValue(x, h).IsInteractable == false) continue;
                        var tile = _grid.GetValue(x, h);
                        _grid.SetValue(x, y, tile);
                        _animation.MoveTile(tile, _grid.GridToWorld(x, y), Ease.InBack);
                        _grid.SetValue(x, h, null);
                        _tilesToRefillPos.Add(new Vector2Int(x, h));
                        break;
                    }
                }
            }
            // play sound
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), _cts.IsCancellationRequested);
            _cts.Cancel();
        }

        private async UniTask CreateEmptyTiles()
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (_grid.GetValue(x, y) != null) continue;
                    var tile = _tilePool.GetTile(_grid.GridToWorld(x,y), _parent);
                    _grid.SetValue(x, y, tile);
                    tile.gameObject.SetActive(true);
                    await _animation.Reveal(tile.gameObject, 0.1f);
                }
            }
        }

        private void CheckEndGame()
        {
            _stateSwitcher.SwitchState<PlayerTurnState>();
        }

        public void Exit() => _cts?.Cancel();
    }
}