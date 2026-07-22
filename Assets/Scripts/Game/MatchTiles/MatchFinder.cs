using System.Collections.Generic;
using System.Linq;
using Game.Tiles;
using ResurcesLoading;
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
        public List<BlankTile> BlankTilesToRemove { get; }
        public List<Tile> VerticalRocketTilesToRemove { get; }
        public List<Tile> HorizontalRocketTilesToRemove { get; }
        public MatchResult CurrentMatchResult { get; private set; }
        
        private GameResurcesLoader _resurcesLoader;
        public MatchFinder(GameResurcesLoader resurcesLoader)
        {
            TilesToRemove = new List<Tile>();
            BlankTilesToRemove =  new List<BlankTile>();
            VerticalRocketTilesToRemove =  new List<Tile>();
            HorizontalRocketTilesToRemove =  new List<Tile>();
            _resurcesLoader = resurcesLoader;
        }
        
        public bool CheckBoardForMatches(Grid grid)
        {
            var hasMatched = false;
            ClearAnyTilesToRemove();
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
        
        public void ClearAnyTilesToRemove()
        {
            foreach (var tile in TilesToRemove)
            {
                tile.SetMatch(false);
            }
            
            TilesToRemove.Clear();
            BlankTilesToRemove.Clear();
            VerticalRocketTilesToRemove.Clear();
            HorizontalRocketTilesToRemove.Clear();
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
        }//
        
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
                    tile.TileConfig == neighbourTile.TileConfig 
                    || neighbourTile.TileConfig.TileKind == TileKind.RocketVertical
                    || neighbourTile.TileConfig.TileKind == TileKind.RocketHorizontal
                    || neighbourTile.TileConfig.TileKind == TileKind.Bomb) 
                {
                    connectedTiles.Add(neighbourTile);
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
                var tileGridPos = grid.WorldToGrid(tile.transform.position);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.left);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.right);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.up);
                CheckDirectionOnBlankTiles(grid, tileGridPos, Vector2Int.down);
                
            }
        }

        private void CheckDirectionOnBlankTiles(Grid grid,Vector2Int position, Vector2Int direction)
        {
            var checkedPos = position + direction;
            if (!grid.IsValidPosition(checkedPos.x, checkedPos.y)) return;
                
            var tile = grid.GetValue(checkedPos.x,checkedPos.y);
            if (tile.tileKind == TileKind.Blank)
            {
                BlankTile blankTile = (BlankTile)tile;
                //blankTile.ChangeState(_resurcesLoader);
                if (!BlankTilesToRemove.Contains(blankTile)) // ПОМОЕМУ ЭТО ВООБЩЕ НЕ НУЖНО
                //(СПИСОК ОТЧИЩАЕТСЯ ВСЕГДА) И НАПИСАН ТУТ БРЕД.  
                    BlankTilesToRemove.Add(blankTile);
            }
        }
        #endregion

        #region RocketTileLogic
        public void CheckOnRocketTiles(Grid grid)
        {
            foreach (var tile in TilesToRemove.ToList())
            {
                if (tile.tileKind == TileKind.RocketVertical)
                {
                    VerticalRocketTilesToRemove.Add(tile);
                    TilesToRemove.Remove(tile);
                }
                else if (tile.tileKind == TileKind.RocketHorizontal)
                {
                    HorizontalRocketTilesToRemove.Add(tile);
                    TilesToRemove.Remove(tile);
                }
            }
        }
        
        private List<Tile> CheckDirectionRocketLine(Grid grid, Vector2Int rocketPos, GameResurcesLoader resurcesLoader)
        {
            List<Tile> tiles = new List<Tile>();
            for (int i = rocketPos.x; i < grid.Width; i+=rocketPos.x)
            {
                for (int j = rocketPos.y; j < grid.Height; j++)
                {
                    var tile = grid.GetValue(i, j);
                    if (tile ==null) break;
                    //if (tile.tileKind == TileKind.Rocket) continue;
                    
                    tiles.Add(tile);
                    /*if (tile.tileKind == TileKind.Blank)
                    {
                        var blankTile = (BlankTile)tile;
                        blankTile.ChangeState(resurcesLoader);
                    }
                    else if (tile.tileKind == TileKind.Normal)
                    {

                    }*/
                }
            }
            return tiles;
        }
        #endregion
    }
}