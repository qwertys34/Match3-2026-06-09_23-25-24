using System.Collections.Generic;
using ResurcesLoading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Tiles
{
    public class TilePool
    {
        private List<Tile> _tilePool = new();
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
            
            var newTile = CreateTile<Tile>(position, parent);
            return newTile;
        }

        public Tile CreateTile<T>(Vector3 position, Transform parent) where T : Tile
        {
            var tilePrefab = _objectResolver.Instantiate(_gameResurcesLoader.TilePrefab,
                position, Quaternion.identity, parent);
            T tile = tilePrefab.AddComponent<T>();
            switch (typeof(T).Name)
            {
                case nameof(Tile):
                    tile.SetTileConfig(GetRandomTileConfig());
                    tile.SetTileKind(TileKind.Normal);
                    break;
                case nameof(BlankTile):
                    tile.SetTileConfig(_gameResurcesLoader.BlankConfig);
                    tile.SetTileKind(TileKind.Blank);
                    break;
                case nameof(VerticalRocketTile):
                    tile.SetTileConfig(_gameResurcesLoader.VerticalRocketConfig);
                    tile.SetTileKind(TileKind.RocketVertical);
                    break;
                case nameof(HorizontalRocketTile):
                    tile.SetTileConfig(_gameResurcesLoader.HorizontalRocketConfig);
                    tile.SetTileKind(TileKind.RocketHorizontal);
                    break;
                case nameof(BombTile):
                    tile.SetTileConfig(_gameResurcesLoader.BombConfig);
                    tile.SetTileKind(TileKind.Bomb);
                    break;
                case  nameof(JellyTile):
                    tile.SetTileConfig(GetRandomTileConfig());
                    tile.SetTileKind(TileKind.Jelly); // чтобы получить, что тайл jelly нужно всегда брать поле tileKind 
                    var jellyPrefab = _objectResolver.Instantiate(_gameResurcesLoader.TilePrefab,
                        position, Quaternion.identity, tilePrefab.transform);
                    jellyPrefab.GetComponent<SpriteRenderer>().sprite = _gameResurcesLoader.JellyTileSpriteOne;
                    tile.JellyTransform = jellyPrefab.transform;
                    break;
                default: Debug.Log("Nothink such tile");
                    break;
            }
            return  tile;
        }

        private TileConfig GetRandomTileConfig() => _gameResurcesLoader.CurrentTileSet[Random
            .Range(0, _gameResurcesLoader.CurrentTileSet.Count)];
    }
}