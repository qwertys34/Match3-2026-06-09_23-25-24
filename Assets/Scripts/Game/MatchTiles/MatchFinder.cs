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
        public List<Tile> TilesToRemove { get; }
        public MatchResult CurrentMatchResult { get; private set; }

        public MatchFinder() => TilesToRemove = new List<Tile>();

        public bool CheckBoardForMatches(Grid grid)
        {
            var hasMatched = false;
            ClearTilesToRemove();
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var tile =  grid.GetValue(x, y);
                    if (tile == null) continue;
                    if (tile.IsMatched || tile.IsInteractable == false) continue; 
                    MatchResult matchTiles = FindConnectedTiles(tile, grid);
                    if (matchTiles.ConectedTiles.Count < 3) continue;
                    
                    CurrentMatchResult = matchTiles;
                    TilesToRemove.AddRange(matchTiles.ConectedTiles);
                    foreach (var connectedTile in matchTiles.ConectedTiles) 
                        connectedTile.SetMatch(true);
                    hasMatched = true;
                }
            }
            return hasMatched;
        }
        
        public void ClearTilesToRemove()
        {
            foreach (var tile in TilesToRemove) 
                tile.SetMatch(false);
            
            TilesToRemove.Clear();
        }
        
        public void ClearCurrentMatchResult() => 
            CurrentMatchResult.ConectedTiles.Clear();

        public MatchResult FindConnectedTiles(Tile tile, Grid grid)
        {
            List<Tile> connectedTiles = new List<Tile>();
            connectedTiles.Add(tile);
            var tileGridPos = grid.WorldToGrid(tile.transform.position);
            
            CheckDirection(tileGridPos, Vector2Int.right, grid, tile,  connectedTiles);
            CheckDirection(tileGridPos, Vector2Int.left, grid, tile,  connectedTiles);
            if (connectedTiles.Count == 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.up,
                    MatchDirection.Horizontal); 
            if (connectedTiles.Count > 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.up,
                    MatchDirection.LongHorizontal); 
            
            connectedTiles.Clear();
            connectedTiles.Add(tile);
            CheckDirection(tileGridPos, Vector2Int.up, grid, tile,  connectedTiles);
            CheckDirection(tileGridPos, Vector2Int.down, grid, tile,  connectedTiles);
            if (connectedTiles.Count == 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.right,
                    MatchDirection.Vertical);
            if (connectedTiles.Count > 3)
                return CheckForMultiResult(connectedTiles, grid, Vector2Int.right,
                    MatchDirection.LongVertical);
            
            connectedTiles.Clear();
            return new MatchResult(connectedTiles, MatchDirection.None);
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
               if (neighbourTile.IsInteractable && neighbourTile.IsMatched == false &&
                   tile.TileConfig == neighbourTile.TileConfig) 
               {
                   connectedTiles.Add(neighbourTile);
                   x += direction.x;
                   y += direction.y;
               }
               else 
                   break;
            }
            
        }

        private MatchResult CheckForMultiResult(List<Tile> connectedTiles, Grid grid,
            Vector2Int direction, MatchDirection matchDirection)
        {
            foreach (var tile in connectedTiles)
            {
                var tilePos =  grid.WorldToGrid(tile.transform.position);
                var multiConnectedTiles = new List<Tile>();
                multiConnectedTiles.Add(tile); 
                CheckDirection(tilePos, direction, grid, tile, multiConnectedTiles);
                CheckDirection(tilePos, direction * -1, grid, tile, multiConnectedTiles);
                if (multiConnectedTiles.Count <= 2) continue;
                multiConnectedTiles.AddRange(connectedTiles);
                return new MatchResult(multiConnectedTiles, MatchDirection.Multiply); 
            }
            
            return new MatchResult(connectedTiles, matchDirection);
        }
    }
}