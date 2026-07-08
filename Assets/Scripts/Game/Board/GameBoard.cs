using System.Collections.Generic;
using Animations;
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
        private BlankTilesSetup _blankTilesSetup;
        private IAnimation _animation;
        private MatchFinder _matchFinder;
        
        [Inject] private void Construct(Grid grid, TilePool tilePool,
            BlankTilesSetup blankTilesSetup, IAnimation animation, MatchFinder matchFinder)
        {
            _grid = grid;
            _tilePool = tilePool;
            _animation = animation;
            _blankTilesSetup = blankTilesSetup;
            _matchFinder = matchFinder;
        }
        

        private void RevealTiles()
        {
            foreach (var tile in _tilesToRefill)
            {
                var objTile = tile.gameObject;
                _animation.Reveal(objTile, 1f);
            }
        }

        public void CreateBoard()
        {
            FillBoard();
            while (_matchFinder.CheckBoardForMatches(_grid))
            {
                ClearBoard();
                FillBoard();
                Debug.Log("Created board");
            }
            _matchFinder.ClearTilesToRemove();
            RevealTiles();
        }

        private void ClearBoard()
        {
            if (_tilesToRefill == null) return;
            foreach (var tile in _tilesToRefill)
            {
                _grid.SetValue(tile.transform.position, null);
                tile.gameObject.SetActive(false);
            }
            _tilesToRefill.Clear();
        }

        private void FillBoard()
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (_blankTilesSetup.Blanks[x, y])
                    {
                        if (_grid.GetValue(x, y)) continue;
                        
                        var blankTile = _tilePool.CreateBlankTile(_grid.GridToWorld(x, y), transform);
                        _grid.SetValue(x, y, blankTile);
                    }
                    else
                    {
                        var tile = _tilePool.GetTile(_grid.GridToWorld(x, y), transform);
                        _grid.SetValue(x, y, tile);
                        tile.gameObject.SetActive(true);
                        _tilesToRefill.Add(tile);   
                    }
                    

                     
                }
            }   
        }
    }
}