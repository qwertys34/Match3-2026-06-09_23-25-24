using System.Collections.Generic;
using ResurcesLoading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Tiles
{
    public class TilePool
    {
        private List<Tile> _tilePool = new List<Tile>();
        private IObjectResolver _objectResolver;
        private GameResurcesLoader _gameResurcesLoader;
        
        public TilePool(IObjectResolver objectResolver,  GameResurcesLoader gameResurcesLoader)
        {
            _objectResolver = objectResolver;
            _gameResurcesLoader = gameResurcesLoader;
        }

        public Tile GetTile(Vector3 position, Transform parent)
        {
            for (int i = 0; i < _tilePool.Count; i++)
            {
                if (_tilePool[i].gameObject.activeInHierarchy) continue;
                    
                _tilePool[i].SetTileConfig(GetRandomTileConfig());
                _tilePool[i].transform.position = position;
                _tilePool[i].transform.SetParent(parent);
                return _tilePool[i];
            }
            
            var newTile = CreateTile(position, parent);
            return newTile;
        }
        
        private Tile CreateTile(Vector3 position, Transform parent)
        {
            var tilePrefab = _objectResolver.Instantiate(_gameResurcesLoader.TilePrefab,
                position, Quaternion.identity, parent);
            var tile = tilePrefab.GetComponent<Tile>();
            tile.SetTileConfig(GetRandomTileConfig());
            _tilePool.Add(tile);
            return tile;
        }

        private TileConfig GetRandomTileConfig() => _gameResurcesLoader.TileSetConfig
            .Set[Random.Range(0, _gameResurcesLoader.TileSetConfig.Set.Count)];
    }
}