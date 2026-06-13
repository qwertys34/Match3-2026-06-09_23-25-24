using System.Collections.Generic;
using Animations;
using Game.GridSystem;
using Game.Tiles;
using Game.Utils;
using Input;
using Levels;
using UnityEngine;
using VContainer;
using Grid = Game.GridSystem.Grid;

namespace Game.Board
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private bool isVertical;
        [SerializeField] private LevelConfig levelConfig;
        
        [SerializeField] private TileConfig tileConfig;
        [SerializeField] private GameObject gridPrefab;

        [SerializeField] private bool isDebugging;
        
        private TilePool _tilePool;
        private readonly List<Tile> _tilesToRefill = new List<Tile>();
        private Grid _grid;
        private SetupCamera _setupCamera;
        private GameDebug _gameDebug;
        private BlankTilesSetup _blankTilesSetup;
        private IAnimation _animation;
        
        private InputReader _inputs;
        
        [Inject] 
        private void Construct(Grid grid, SetupCamera setupCamera, TilePool tilePool,  GameDebug gameDebug, BlankTilesSetup blankTilesSetup, IAnimation animation)
        {
            _grid = grid;
            _setupCamera = setupCamera;
            _tilePool = tilePool;
            _gameDebug = gameDebug;
            _blankTilesSetup = blankTilesSetup;
            _animation = animation;
        }

        private void Awake()
        {
            _grid.SetupGrid(levelConfig.Width, levelConfig.Height);
            _blankTilesSetup.SetupBlanks(levelConfig);
            _setupCamera.SetCamera(_grid.Width, _grid.Height, isVertical);

            _inputs = new InputReader();
            _inputs.EnableInput(true);
            _inputs.Click += Test;
            
            if (isDebugging)
                _gameDebug.ShowDebug(transform);
        }

        private void RevealTiles()
        {
            foreach (var tile in _tilesToRefill)
            {
                var objTile = tile.gameObject;
                _animation.Reveal(objTile, 1f);
            }
        }
        
        private void Test()
        {
            Debug.Log(_inputs.Position);
        }
        
        private void OnDisable()
        {
            _inputs.Click -= Test;
        }

        public void CreateBoard()
        {
            FillBoard();
            RevealTiles();
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