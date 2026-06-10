using System;
using System.Collections.Generic;
using Game.Tiles;
using UnityEngine;
using VContainer;
using Grid = Game.GridSystem.Grid;

namespace Game.Board
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private TileConfig tileConfig;
        [SerializeField] private GameObject gridPrefab;
        
        private readonly List<Tile> _tilesToRefill = new List<Tile>();
        private Grid _grid;
        
        [Inject] 
        private void Construct(Grid grid)
        {
            _grid = grid;
        }

        private void Start()
        {
            _grid.SetupGrid(10, 10);
            CreateBoard();
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

                    var tile = Instantiate(gridPrefab, transform);
                    tile.transform.position = _grid.GridToWorld(x, y);
                    var tileComponent = tile.GetComponent<Tile>();
                    tileComponent.SetTileConfig(tileConfig);
                    _grid.SetValue(x, y, tileComponent);
                }
            }   
        }
    }
}