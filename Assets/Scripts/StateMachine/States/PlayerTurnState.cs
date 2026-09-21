using System;
using Animations;
using Audio;
using Game.Tiles;
using Input;
using UnityEngine;
using Grid = Game.GridSystem.Grid;

namespace StateMachine.States
{
    public class PlayerTurnState : IState, IDisposable
    {
        private readonly Vector2Int _emptyPosition = Vector2Int.one * -1;
        private readonly IStateSwitcher _stateSwitcher;
        private readonly Grid _grid;
        private readonly InputReader _inputReader;
        private readonly IAnimation _animation;
        private AudioManager _audioManager;

        public PlayerTurnState(IStateSwitcher stateSwitcher, IAnimation animation, Grid grid, AudioManager audioManager)
        {
            _stateSwitcher = stateSwitcher;
            _animation = animation;
            _grid = grid;
            _audioManager = audioManager;
            _inputReader = new InputReader();
            
            _inputReader.Click += OnTileClick; 
            _inputReader.Swipe += OnSwiped;
        }
        
        public void Enter()
        {
            _inputReader.EnablePlayerInput(true);
            DeselectTile();
        }

        private void OnSwiped(Vector2 startPos, Vector2 endPos) 
        {
            DeselectTile();
            var camera = Camera.main;
            var startGridPos = _grid.WorldToGrid(camera.ScreenToWorldPoint(startPos));
            var endGridPos = _grid.WorldToGrid(camera.ScreenToWorldPoint(endPos));

            var dir = new Vector2(endGridPos.x - startGridPos.x, endGridPos.y - startGridPos.y).normalized;
            var dirInt = new Vector2Int((int)dir.x, (int)dir.y);
            var newPos = startGridPos + dirInt;
            
            if (!IsValidPosition(startGridPos) || !IsValidPosition(endGridPos))
                return;
            
            if (IsBlankPosition(startGridPos) || IsBlankPosition(endGridPos))
                return;
            
            if (IsSwappable(startGridPos, newPos))
            {
                if (_grid.GetValue(startGridPos.x, startGridPos.y).TileConfig.TileKind == TileKind.Normal &&
                    _grid.GetValue(newPos.x, newPos.y).TileConfig.TileKind == TileKind.SuperCandy)
                {
                    _audioManager.PlayClick();
                    _grid.SetCurrentPosition(startGridPos);
                    _grid.SetTargetPosition(newPos);
                    _animation.AnimateTile(_grid.GetValue(newPos.x, newPos.y), 1f);
                    _stateSwitcher.SwitchState<MergeTilesState>();
                }
                else
                {
                    _audioManager.PlayClick();
                    _grid.SetCurrentPosition(startGridPos);
                    _grid.SetTargetPosition(newPos);
                    _animation.AnimateTile(_grid.GetValue(newPos.x, newPos.y), 1f);
                    _stateSwitcher.SwitchState<SwapTilesState>();
                }
                
            }
            else
            {
                _audioManager.PlayClick(); // Звук ошибки
                DeselectTile();
            }
        }

        private void OnTileClick()
        {
            var camera = Camera.main;
            var clickPosition = _grid.WorldToGrid(
                camera.ScreenToWorldPoint(_inputReader.GetPosition()));

            if (!IsValidPosition(clickPosition) || IsBlankPosition(clickPosition))
            {
                DeselectTile();
                return;
            }
            
            if (_grid.CurrentPosition == _emptyPosition)
            {
                _audioManager.PlayClick();
                _grid.SetCurrentPosition(clickPosition);
                var tile = _grid.GetValue(clickPosition.x, clickPosition.y);
                _animation.AnimateTile(tile, 1.2f);
            }
            else if (_grid.CurrentPosition == clickPosition)
            {
                _audioManager.PlayClick();
                DeselectTile();
            }
            else if (_grid.CurrentPosition != clickPosition && IsSwappable(_grid.CurrentPosition, clickPosition))
            {
                if (_grid.GetValue(_grid.CurrentPosition.x, _grid.CurrentPosition.y).TileConfig.TileKind == TileKind.Normal &&
                    _grid.GetValue(clickPosition.x, clickPosition.y).TileConfig.TileKind == TileKind.SuperCandy)
                {
                    _audioManager.PlayClick();
                    _grid.SetTargetPosition(clickPosition);
                    _animation.AnimateTile(_grid.GetValue(clickPosition.x, clickPosition.y), 1f);
                    _stateSwitcher.SwitchState<MergeTilesState>();
                }
                else
                {
                    _audioManager.PlayClick();
                    _grid.SetTargetPosition(clickPosition);
                    _animation.AnimateTile(_grid.GetValue(clickPosition.x, clickPosition.y), 1f);
                    _stateSwitcher.SwitchState<SwapTilesState>();
                }
            }
            else
            {
                _audioManager.PlayClick();
                DeselectTile();
            }
        }
        
        public void Dispose()
        {
            _inputReader.Click -= OnTileClick;
            _inputReader.Swipe -= OnSwiped;
            _inputReader.Dispose();
        }

        private void DeselectTile()
        {
            if (_grid.CurrentPosition != _emptyPosition)
            {
                _animation.AnimateTile(_grid.GetValue(_grid.CurrentPosition.x, _grid.CurrentPosition.y), 1f);
            }
            _grid.SetCurrentPosition(_emptyPosition);
            _grid.SetTargetPosition(_emptyPosition);
        }

        private bool IsSwappable(Vector2Int currentTilePos, Vector2Int targetTilePos) => 
            Mathf.Abs(currentTilePos.x - targetTilePos.x) 
            + Mathf.Abs(currentTilePos.y - targetTilePos.y) == 1;

        private bool IsBlankPosition(Vector2Int gridPos) => 
            _grid.GetValue(gridPos.x, gridPos.y).TileConfig.TileKind == TileKind.Blank;

        private bool IsValidPosition(Vector2Int gridPos) => 
            gridPos.x >= 0 && gridPos.x < _grid.Width 
            && gridPos.y >= 0 && gridPos.y < _grid.Height;

        public void Exit() => _inputReader.EnablePlayerInput(false);
    }
}