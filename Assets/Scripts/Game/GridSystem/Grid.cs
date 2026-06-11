using Game.Tiles;
using UnityEngine;

namespace Game.GridSystem
{
    public class Grid
    {
        public Tile[,] GameGrid { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2Int CurrentPosition { get; private set; }
        public Vector2Int TargetPosition { get; private set; }
        
        public void SetupGrid(int width, int height)
        {
            Width = width;
            Height = height;
            GameGrid = new Tile[width, height];
        }
        
        public Vector2Int SetCurrentPosition(Vector2Int value) => CurrentPosition = value;
        public Vector2Int SetTargetPosition(Vector2Int value) => TargetPosition = value;

        public Vector3 GridToWorld(int x, int y) => new Vector3(x, y, 0);

        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            return Vector2Int.FloorToInt(new Vector2(worldPosition.x, worldPosition.y));
        }

        public void SetValue(int x, int y, Tile tile)
        {
            if (IsValidPosition(x, y))
                GameGrid[x, y] = tile;
        }
        
        public void SetValue(Vector3 worldPosition, Tile tile)
        {
            var worldToGrid = WorldToGrid(worldPosition);
            SetValue(worldToGrid.x, worldToGrid.y, tile);
        }

        public Tile GetValue(int x, int y) => IsValidPosition(x, y) ? GameGrid[x, y] : default;

        public Tile GetValue(Vector3 worldPosition)
        {
            var worldToGrid = WorldToGrid(worldPosition);
            return GetValue(worldToGrid.x, worldToGrid.y);
        }
        
        public bool IsValidPosition(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height; 
    }
}