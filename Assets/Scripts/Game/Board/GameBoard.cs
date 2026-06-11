using System.Collections.Generic;
using Game.Tiles;
using Game.Utils;
using UnityEngine;
using VContainer;
using Grid = Game.GridSystem.Grid;

namespace Game.Board
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private bool isVertical;
        
        [SerializeField] private TileConfig tileConfig;
        [SerializeField] private GameObject gridPrefab;
        
        private TilePool _tilePool;
        private readonly List<Tile> _tilesToRefill = new List<Tile>();
        private Grid _grid;
        private SetupCamera _setupCamera;
        
        [Inject] 
        private void Construct(Grid grid, SetupCamera setupCamera, TilePool tilePool)
        {
            _grid = grid;
            _setupCamera = setupCamera;
            _tilePool = tilePool;
        }

        private void Start()
        {
            _grid.SetupGrid(10, 10);
            CreateBoard();
            _setupCamera.SetCamera(_grid.Width, _grid.Height, isVertical);
        }

        public void CreateBoard()
        {
            FillBoard();
        }

        private void FillBoard()
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (_grid.GetValue(x, y)) continue;

                    var tile = _tilePool.GetTile(_grid.GridToWorld(x, y), transform);
                    _grid.SetValue(x, y, tile);
                    tile.gameObject.SetActive(true);
                    _tilesToRefill.Add(tile);    
                }
            }   
        }
    }
}