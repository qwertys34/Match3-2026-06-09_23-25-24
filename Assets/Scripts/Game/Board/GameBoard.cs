using System;
using System.Collections.Generic;
using Animations;
using Cysharp.Threading.Tasks;
using Game.MatchTiles;
using Game.Tiles;
using UnityEngine;
using VContainer;
using Grid = Game.GridSystem.Grid;

namespace Game.Board
{
    public class GameBoard : MonoBehaviour
    {
        private readonly List<Tile> _tilesToRefill = new();
        
        private Grid _grid;
        private TilePool _tilePool;
        private InteractablesTilesSetup interactablesTilesSetup;
        private IAnimation _animation;
        private MatchFinder _matchFinder;
        
        [Inject] public void Construct(Grid grid, TilePool tilePool,
            InteractablesTilesSetup interactablesTilesSetup, IAnimation animation, MatchFinder matchFinder)
        {
            _grid = grid;
            _tilePool = tilePool;
            _animation = animation;
            this.interactablesTilesSetup = interactablesTilesSetup;
            _matchFinder = matchFinder;
        }

        public async UniTask CreateBoard()
        {
            //int maxIterations = 1000;
            int iteration = 0;
            
            // Фаза 1: Быстрая генерация без анимаций
            do
            {
                ClearBoard();
                FillBoardSync();
                
                // Даём кадр движку каждые 100 итераций
                if (++iteration % 100 == 0)
                    await UniTask.Yield(PlayerLoopTiming.Update);
                    
            } while (_matchFinder.CheckBoardForMatches(_grid) /*&& iteration < maxIterations*/);
            
            _matchFinder.ClearAnyTilesToRemove();
            
            // Фаза 2: Красивая анимация появления тайлов
            await RevealTilesAsync();
        }

        private void ClearBoard()
        {
            if (_tilesToRefill == null) return;
            
            foreach (var tile in _tilesToRefill)
            {
                if (tile == null) continue;
                _grid.SetValue(tile.transform.position, null);
                tile.gameObject.SetActive(false);
            }
            
            _tilesToRefill.Clear();
        }
        
        private void FillBoardSync()
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    var tileKind = interactablesTilesSetup.tileKind[x, y];
                    var worldPos = _grid.GridToWorld(x, y);
                    
                    switch (tileKind)
                    {
                        case TileKind.Blank:
                            if (_grid.GetValue(x, y)) continue;
                            var blankTile = _tilePool.GetTile<BlankTile>(worldPos, transform);
                            _grid.SetValue(x, y, blankTile);
                            _tilesToRefill.Add(blankTile);
                            break;
                            
                        case TileKind.RocketVertical:
                            var verticalRocketTile = _tilePool.GetTile<VerticalRocketTile>(worldPos, transform);
                            _grid.SetValue(x, y, verticalRocketTile);
                            _tilesToRefill.Add(verticalRocketTile);
                            break;
                            
                        case TileKind.RocketHorizontal:
                            var horizontalRocketTile = _tilePool.GetTile<HorizontalRocketTile>(worldPos, transform);
                            _grid.SetValue(x, y, horizontalRocketTile);
                            _tilesToRefill.Add(horizontalRocketTile);
                            break;
                            
                        case TileKind.Jelly:
                            var jellyTile = _tilePool.GetTile<JellyTile>(worldPos, transform);
                            if (jellyTile == null)
                            {
                                Debug.LogError("Jelly tile is null");
                                continue;
                            }
                            _grid.SetValue(x, y, jellyTile);
                            _tilesToRefill.Add(jellyTile);
                            break;
                            
                        case TileKind.Bomb:
                            var bombTile = _tilePool.GetTile<BombTile>(worldPos, transform);
                            _grid.SetValue(x, y, bombTile);
                            _tilesToRefill.Add(bombTile);
                            break;
                            
                        case TileKind.SuperCandy:
                            var superCandyTile = _tilePool.GetTile<SuperCandyTile>(worldPos, transform);
                            _grid.SetValue(x, y, superCandyTile);
                            _tilesToRefill.Add(superCandyTile);
                            break;
                            
                        case TileKind.Normal:
                            var tile = _tilePool.GetTile<Tile>(worldPos, transform);
                            _grid.SetValue(x, y, tile);
                            _tilesToRefill.Add(tile);
                            break;
                            
                        default:
                            Debug.LogWarning($"Unknown tile kind: {tileKind}");
                            break;
                    }
                }
            }
        }

        private async UniTask RevealTilesAsync()
        {
            for (int i = 0; i < _tilesToRefill.Count; i++)
            {
                var tile = _tilesToRefill[i];
                if (tile == null) continue;
                
                tile.gameObject.SetActive(true);
                
                switch (tile.tileKind)
                {
                    case TileKind.Blank:
                        _ = _animation.Reveal(tile.gameObject, 1f);
                        break;
                        
                    case TileKind.RocketVertical:
                    case TileKind.RocketHorizontal:
                    case TileKind.Bomb:
                    case TileKind.SuperCandy:
                        _ = _animation.Reveal(tile.gameObject, 1f);
                        break;
                        
                    case TileKind.Jelly:
                        var jellyTile = tile as JellyTile;
                        if (jellyTile != null)
                        {
                            _ = _animation.Reveal(tile.gameObject, 1f);
                            _ = _animation.Reveal(jellyTile.JellyTransform.gameObject, 1f);
                        }
                        break;
                        
                    case TileKind.Normal:
                        _ = _animation.Reveal(tile.gameObject, 1f);
                        // Обычные тайлы просто появляются без анимации
                        break;
                }
                
                // Задержка для эффекта "волны" (каждые 5 тайлов или каждый тайл)
                if (i % 3 == 0)
                    await UniTask.Delay(TimeSpan.FromMilliseconds(15));
            }
            
            // Даём последний кадр для завершения анимаций
            await UniTask.Delay(TimeSpan.FromMilliseconds(50));
        }
    }
}