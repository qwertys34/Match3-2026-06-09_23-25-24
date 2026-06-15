using System;
using Animations;
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
        private readonly Camera _camera;
        private readonly InputReader _inputReader;
        private readonly IAnimation _animation;
        
        public PlayerTurnState(IStateSwitcher stateSwitcher, IAnimation animation, Grid grid)
        {
            _stateSwitcher = stateSwitcher;
            _animation = animation;
            _grid = grid;
            _camera = Camera.main;
            _inputReader = new InputReader();
            _inputReader.Click += OnTileClick;
        }

        private void OnTileClick()
        {
            var clickPosition = _grid.WorldToGrid(
                _camera.ScreenToWorldPoint(_inputReader.Position()));

            if (!IsValidPosition(clickPosition) || IsBlankPosition(clickPosition))
                return;
            if (_grid.CurrentPosition == _emptyPosition)
            {
                // play sound
                _grid.SetCurrentPosition(clickPosition);
                _animation.AnimateTile(_grid.GetValue(clickPosition.x, clickPosition.y), 1.2f);
            }
            else if (_grid.CurrentPosition == clickPosition)
            {
                // play sound
                DeselectTile();
            }
            else if (_grid.CurrentPosition != clickPosition && IsSwappable(
                         _grid.CurrentPosition, clickPosition))
            {
                _grid.SetTargetPosition(clickPosition);
                _animation.AnimateTile(_grid.GetValue(clickPosition.x, clickPosition.y), 1f);
                _stateSwitcher.SwitchState<SwapTilesState>();
            }

        }

        public void Dispose()
        {
            _inputReader.Click -= OnTileClick;
        }

        public void Enter()
        {
            _inputReader.EnableInput(true);
            DeselectTile();
        }

        private void DeselectTile()
        {
            _animation.AnimateTile(_grid.GetValue(_grid.CurrentPosition.x, _grid.CurrentPosition.y), 1f);
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
            &&  gridPos.y >= 0 && gridPos.y < _grid.Height;

        public void Exit()
        {
            _inputReader.EnableInput(false);
        }
    }
}