using System;
using System.Collections.Generic;
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
    public class RefillGridState : IState, IDisposable
    {
        private IStateSwitcher _stateSwitcher;
        private Grid _grid;
        private IAnimation _animation;
        private CancellationTokenSource _cts;
        private MatchFinder _matchFinder;
        private TilePool _tilePool;
        private Transform _parent;
        private GameProgress _gameProgress;
        private AudioManager _audioManager;

        private List<Vector2Int> _tilesToRefillPos = new List<Vector2Int>(); 
        
        public RefillGridState(IStateSwitcher switcher, Grid grid, IAnimation animation, MatchFinder matchFinder,
            TilePool tilePool, Transform parent,  GameProgress gameProgress, AudioManager audioManager)
        {
            _stateSwitcher = switcher;
            _grid = grid;
            _animation = animation;
            _matchFinder = matchFinder;
            _tilePool = tilePool;
            _parent = parent;
            _gameProgress = gameProgress;
            _audioManager =  audioManager;
        }

        public void Dispose() => _cts?.Dispose();

        public async void Enter()
        {
            await TileFall();
            await RefillTiles();
            if (_matchFinder.CheckBoardForMatches(_grid))
            {
                _matchFinder.CheckToBlankTiles(_grid);
                _stateSwitcher.SwitchState<RemoveTileState>();
                _audioManager.PlayMatch();
            }
            else
            {
                CheckEndGame();
                _audioManager.PlayNoMatch();
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
            _audioManager.PlayWhoosh();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), _cts.IsCancellationRequested);
            _cts.Cancel();
        }

        private async UniTask RefillTiles()
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
                    _audioManager.PlayPop();
                    await UniTask.Delay(TimeSpan.FromSeconds(0.05f), _cts.IsCancellationRequested);
                }
            }
        }

        private void CheckEndGame()
        {
            var check = _gameProgress.CheckGoalScore();
            if (check)
                _stateSwitcher.SwitchState<WinState>();
            else if (_gameProgress.Moves <= 0)
                _stateSwitcher.SwitchState<LooseState>();
            else 
                _stateSwitcher.SwitchState<PlayerTurnState>();
        }

        public void Exit() => _cts?.Cancel();
    }
}