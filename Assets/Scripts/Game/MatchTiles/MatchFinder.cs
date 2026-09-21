using System.Collections.Generic;
using Game.Tiles;
using UnityEngine;
using Grid = Game.GridSystem.Grid;

namespace Game.MatchTiles
{
    public enum MatchDirection
    {
        Horizontal,
        Vertical,
        LongHorizontal,
        LongVertical,
        Multiply,
        None
    }
    
    public class MatchFinder
    {
        public List<Tile> TilesToRemove { get; } = new();
        public List<BlankTile> BlankTilesToRemove { get; } = new();
        public List<Tile> RocketTilesToRemove { get; } = new();
        public MatchResult CurrentMatchResult { get; private set; }

        public bool CheckBoardForMatches(Grid grid)
        {
            var hasMatched = false;
            ClearAnyTilesToRemove();
            
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var tile = grid.GetValue(x, y);
                    if (tile == null) continue;
                    if (tile.IsMatched || tile.IsInteractable == false) continue;
                    
                    MatchResult matchTiles = FindConnectedTiles(tile, grid);
                    if (matchTiles.ConectedTiles.Count < 3) continue;
                    
                    CurrentMatchResult = matchTiles;
                    
                    // Добавляем только уникальные тайлы
                    foreach (var connectedTile in matchTiles.ConectedTiles)
                    {
                        if (!TilesToRemove.Contains(connectedTile))
                        {
                            TilesToRemove.Add(connectedTile);
                            connectedTile.SetMatch(true);
                        }
                    }
                    hasMatched = true;
                }
            }
            return hasMatched;
        }

        public List<Tile> FindAnyTilesWithSprite(SpriteRenderer sr, Grid grid)
        {
            List<Tile> matchingTiles = new();
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var tile = grid.GetValue(x, y);
                    if (tile != null && tile.GetComponent<SpriteRenderer>().sprite == sr.sprite)
                    {
                        matchingTiles.Add(tile);
                    }
                }
            }
            return matchingTiles;
        }
        
        public void ClearAnyTilesToRemove()
        {
            // Сбрасываем флаги у всех тайлов
            foreach (var tile in TilesToRemove)
            {
                if (tile != null)
                    tile.SetMatch(false);
            }
            
            foreach (var rocket in RocketTilesToRemove)
            {
                if (rocket != null)
                    rocket.SetMatch(false);
            }
            
            // Очищаем все списки
            TilesToRemove.Clear();
            BlankTilesToRemove.Clear();
            ClearRocketTiles();
            ClearCurrentMatchResult();
        }
        
        public void ClearCurrentMatchResult() => CurrentMatchResult?.ConectedTiles.Clear();

        public MatchResult FindConnectedTiles(Tile tile, Grid grid)
        {
            List<Tile> connectedTiles = new List<Tile>();
            connectedTiles.Add(tile);
            var tileGridPos = grid.WorldToGrid(tile.transform.position);
            
            // Проверяем горизонтальное направление
            CheckDirection(tileGridPos, Vector2Int.right, grid, tile, connectedTiles);
            CheckDirection(tileGridPos, Vector2Int.left, grid, tile, connectedTiles);
            
            if (connectedTiles.Count == 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.up, MatchDirection.Horizontal); 
            if (connectedTiles.Count > 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.up, MatchDirection.LongHorizontal); 
            
            // Проверяем вертикальное направление
            connectedTiles.Clear();
            connectedTiles.Add(tile);
            CheckDirection(tileGridPos, Vector2Int.up, grid, tile, connectedTiles);
            CheckDirection(tileGridPos, Vector2Int.down, grid, tile, connectedTiles);
            
            if (connectedTiles.Count == 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.right, MatchDirection.Vertical);
            if (connectedTiles.Count > 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.right, MatchDirection.LongVertical);
            
            connectedTiles.Clear();
            return new MatchResult(connectedTiles, MatchDirection.None);
        }
        
        private MatchResult CheckForMultiResult(List<Tile> connectedTiles, Grid grid,
            Vector2Int direction, MatchDirection matchDirection)
        {
            foreach (var tile in connectedTiles)
            {
                var tilePos = grid.WorldToGrid(tile.transform.position);
                var multiConnectedTiles = new List<Tile>();
                multiConnectedTiles.Add(tile); 
                
                CheckDirection(tilePos, direction, grid, tile, multiConnectedTiles);
                CheckDirection(tilePos, direction * -1, grid, tile, multiConnectedTiles);
                
                if (multiConnectedTiles.Count <= 2) continue;
                
                // Добавляем только уникальные тайлы
                foreach (var connectedTile in connectedTiles)
                {
                    if (!multiConnectedTiles.Contains(connectedTile))
                    {
                        multiConnectedTiles.Add(connectedTile);
                    }
                }
                return new MatchResult(multiConnectedTiles, MatchDirection.Multiply); 
            }
            
            return new MatchResult(connectedTiles, matchDirection);
        }
        
        private void CheckDirection(Vector2Int position, Vector2Int direction,
            Grid grid, Tile tile, List<Tile> connectedTiles)
        {
            var x = position.x + direction.x;
            var y = position.y + direction.y;
            
            while (grid.IsValidPosition(x, y))
            {
                var neighbourTile = grid.GetValue(x, y);
                if (neighbourTile == null) break;
                
                // Проверяем, совместим ли соседний тайл
                bool isCompatible = false;
                
                if (neighbourTile.IsInteractable && !neighbourTile.IsMatched)
                {
                    // Проверяем совпадение по конфигурации
                    if (tile.TileConfig == neighbourTile.TileConfig)
                    {
                        isCompatible = true;
                    }
                    // Или это специальный тайл
                    else if (neighbourTile.TileConfig.TileKind == TileKind.RocketVertical ||
                             neighbourTile.TileConfig.TileKind == TileKind.RocketHorizontal ||
                             neighbourTile.TileConfig.TileKind == TileKind.Bomb)
                    {
                        isCompatible = true;
                    }
                }
                
                if (isCompatible)
                {
                    // Проверяем, не добавлен ли уже этот тайл
                    if (!connectedTiles.Contains(neighbourTile))
                    {
                        connectedTiles.Add(neighbourTile);
                    }
                    x += direction.x;
                    y += direction.y;
                }
                else 
                    break;
            }
        }
        
        #region BlankTileLogic
        public void CheckToBlankTiles(Grid grid)
        {
            foreach (var tile in TilesToRemove)
            {
                if (tile == null) continue;
                
                var tileGridPos = grid.WorldToGrid(tile.transform.position);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.left);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.right);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.up);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.down);
            }
        }

        private void CheckDirectionOnBlankTiles(Grid grid, Vector2Int position, Vector2Int direction)
        {
            var checkedPos = position + direction;
            if (!grid.IsValidPosition(checkedPos.x, checkedPos.y)) return;
            var kind = grid.GetValue(position.x, position.y).tileKind;    
            var tile = grid.GetValue(checkedPos.x, checkedPos.y);
            if (tile != null && tile.tileKind == TileKind.Blank && kind != TileKind.Bomb
                && kind != TileKind.RocketHorizontal && kind != TileKind.RocketVertical) 
            {
                BlankTile blankTile = (BlankTile)tile;
                if (!BlankTilesToRemove.Contains(blankTile))
                {
                    BlankTilesToRemove.Add(blankTile);
                }
            }
        }
        #endregion

        #region RocketTileLogic
        public void CheckOnRocketTiles(Grid grid)
        {
            // Идем с конца списка, чтобы безопасно удалять элементы
            for (int i = TilesToRemove.Count - 1; i >= 0; i--)
            {
                var tile = TilesToRemove[i];
                if (tile == null) continue;
                
                if (tile.tileKind == TileKind.RocketVertical || tile.tileKind == TileKind.RocketHorizontal)
                {
                    if (!RocketTilesToRemove.Contains(tile))
                    {
                        RocketTilesToRemove.Add(tile);
                    }
                    tile.SetMatch(false); // Сбрасываем флаг
                    TilesToRemove.RemoveAt(i);
                }
            }
        }
        
        // Дополнительный метод для очистки Rocket тайлов после их использования
        public void ClearRocketTiles()
        {
            foreach (var horizRocket in RocketTilesToRemove)
            {
                if (horizRocket != null)
                    horizRocket.SetMatch(false);
            }
            
            RocketTilesToRemove.Clear();
        }
        #endregion
    }
}