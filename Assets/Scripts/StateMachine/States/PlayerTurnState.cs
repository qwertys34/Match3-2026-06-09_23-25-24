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
        private readonly Camera _camera;
        private readonly InputReader _inputReader;
        private readonly IAnimation _animation;
        private AudioManager _audioManager;

        public PlayerTurnState(IStateSwitcher stateSwitcher, IAnimation animation, Grid grid, AudioManager audioManager)
        {
            _stateSwitcher = stateSwitcher;
            _animation = animation;
            _grid = grid;
            _audioManager = audioManager;
            _camera = Camera.main;
            _inputReader = new InputReader();
            
            _inputReader.Click += OnTileClick; 
            _inputReader.Swipe += OnSwiped;
        }

        private void OnSwiped(Vector2 startPos, Vector2 endPos) 
        {
            
            var startGridPos = _grid.WorldToGrid(_camera.ScreenToWorldPoint(startPos));
            var endGridPos = _grid.WorldToGrid(_camera.ScreenToWorldPoint(endPos));
            
            // Проверяем валидность позиций
            if (!IsValidPosition(startGridPos) || !IsValidPosition(endGridPos))
            {
                Debug.Log("Свайп за пределами сетки");
                return;
            }
            
            // Проверяем, что это не пустая клетка
            if (IsBlankPosition(startGridPos) || IsBlankPosition(endGridPos))
            {
                Debug.Log("Свайп по пустой клетке");
                return;
            }
            
            // Проверяем, что стартовая и конечная позиция — соседние тайлы
            if (IsSwappable(startGridPos, endGridPos))
            {
                _audioManager.PlayClick();
                _grid.SetCurrentPosition(startGridPos);
                _grid.SetTargetPosition(endGridPos);
                _animation.DoPunchAnimate(_grid.GetValue(startGridPos.x, startGridPos.y).gameObject,Vector3.one* 1.2f,0.2f);
                _stateSwitcher.SwitchState<SwapTilesState>();
            }
            else
            {
                Debug.Log("Свайп не между соседними тайлами");
                _audioManager.PlayClick(); // Звук ошибки
                DeselectTile();
            }
        }

        private void OnTileClick()
        {
            
            var clickPosition = _grid.WorldToGrid(
                _camera.ScreenToWorldPoint(_inputReader.GetPosition()));

            if (!IsValidPosition(clickPosition) || IsBlankPosition(clickPosition))
            {
                DeselectTile();
                return;
            }
            
            // Если ни одна клетка не выбрана
            if (_grid.CurrentPosition == _emptyPosition)
            {
                _audioManager.PlayClick();
                _grid.SetCurrentPosition(clickPosition);
                var tile = _grid.GetValue(clickPosition.x, clickPosition.y);
                _animation.AnimateTile(tile, 1.2f);
                Debug.Log($"Выбрана клетка: {clickPosition}");
            }
            // Если кликнули по уже выбранной клетке — снимаем выделение
            else if (_grid.CurrentPosition == clickPosition)
            {
                _audioManager.PlayClick();
                DeselectTile();
                Debug.Log("Снято выделение");
            }
            // Если кликнули по другой клетке и она соседняя — меняем
            else if (_grid.CurrentPosition != clickPosition && IsSwappable(_grid.CurrentPosition, clickPosition))
            {
                _audioManager.PlayClick();
                _grid.SetTargetPosition(clickPosition);
                _animation.AnimateTile(_grid.GetValue(clickPosition.x, clickPosition.y), 1f);
                _stateSwitcher.SwitchState<SwapTilesState>();
                Debug.Log($"Меняем {_grid.CurrentPosition} с {clickPosition}");
            }
            else
            {
                // Кликнули по несоседней клетке — снимаем выделение
                _audioManager.PlayClick();
                DeselectTile();
                Debug.Log("Клик по несоседней клетке — выделение снято");
            }
        }

        public void Dispose()
        {
            _inputReader.Click -= OnTileClick;
            _inputReader.Swipe -= OnSwiped;
            _inputReader.Dispose();
        }

        public void Enter()
        {
            _inputReader.EnableInput(true);
            DeselectTile();
            Debug.Log("PlayerTurnState Enter");
        }

        private void DeselectTile()
        {
            if (_grid.CurrentPosition != _emptyPosition)
            {
                _animation.AnimateTile(_grid.GetValue(_grid.CurrentPosition.x, _grid.CurrentPosition.y), 1f);
            }
            _grid.SetCurrentPosition(_emptyPosition);
            _grid.SetTargetPosition(_emptyPosition);
            Debug.Log("Тайл снят с выделения");
        }

        private bool IsSwappable(Vector2Int currentTilePos, Vector2Int targetTilePos) => 
            Mathf.Abs(currentTilePos.x - targetTilePos.x) 
            + Mathf.Abs(currentTilePos.y - targetTilePos.y) == 1;

        private bool IsBlankPosition(Vector2Int gridPos) => 
            _grid.GetValue(gridPos.x, gridPos.y).TileConfig.TileKind == TileKind.Blank;

        private bool IsValidPosition(Vector2Int gridPos) => 
            gridPos.x >= 0 && gridPos.x < _grid.Width 
            && gridPos.y >= 0 && gridPos.y < _grid.Height;

        public void Exit()
        {
            _inputReader.EnableInput(false);
            Debug.Log("PlayerTurnState Exit");
        }
    }
}