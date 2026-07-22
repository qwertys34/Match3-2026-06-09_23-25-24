using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Game.Tiles;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResurcesLoading
{
    public class GameResurcesLoader : IDisposable
    {
        // base tile prefabs/configs
        public GameObject TilePrefab { get; private set; }
        public GameObject BackgroundTilePrefab { get; private set; }
        public Sprite BgTileSpriteCandy { get; private set; }
        public GameObject FXRemoveTilePrefab { get; private set; }
        public List<TileConfig> CurrentTileSet { get; private set; }
        
        // blank
        public TileConfig BlankConfig { get; private set; }
        public Sprite BlankTileSpriteOne { get; private set; }
        public Sprite BlankTileSpriteTwo { get; private set; }
        public Sprite BlankTileSpriteThree { get; private set; }
        
        // jelly
        public TileConfig JellyConfig { get; private set; }
        public Sprite JellyTileSpriteOne { get; private set; }
        public Sprite JellyTileSpriteTwo { get; private set; }
        
        // rocket
        public TileConfig VerticalRocketConfig { get; private set; }
        public TileConfig HorizontalRocketConfig { get; private set; }
        
        // bomb
        public TileConfig BombConfig  { get; private set; }
        
        private readonly GameData gameData;
        private CancellationTokenSource _cts;
        
        public GameResurcesLoader(GameData gameData) => this.gameData = gameData;
        
        public event Func<UniTask> LoadComplete;

        public void Dispose() => _cts?.Dispose();
        
        public async UniTask Load()
        {
            await LoadSet();
            await LoadTilesPrefab();
            await LoadSprits();
            LoadComplete?.Invoke();
        }

        private async UniTask LoadSet()
        {
            CurrentTileSet = new List<TileConfig>();
            
            CurrentTileSet = (await Loader<TileSetConfig>("Candy")).Set;
            
            BlankConfig = await Loader<TileConfig>("BlankConfig");
            VerticalRocketConfig = await Loader<TileConfig>("VerticalRocketConfig");
            HorizontalRocketConfig = await Loader<TileConfig>("HorizontalRocketConfig");
            JellyConfig = await Loader<TileConfig>("JellyConfig");
            BombConfig = await Loader<TileConfig>("BombConfig");
        }

        private async UniTask<T> Loader<T>(string key)
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
            await asyncOperationHandle.ToUniTask();
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var result = asyncOperationHandle.Result;
                Addressables.Release(asyncOperationHandle);
                return result;
            }
            return default;
        }

        public async UniTask<Sprite> CreateBlankSprite(int state)
        {
            return state switch
            {
                1 => await Loader<Sprite>("BlankTileSpriteOne"),
                2 => await Loader<Sprite>("BlankTileSpriteTwo"),
                3 => await Loader<Sprite>("BlankTileSpriteThree"),
                _ => null
            };
        }
        public async UniTask<Sprite> CreateJellySprite(int state)
        {
            return state switch
            {
                1 => await Loader<Sprite>("JellyTileSpriteOne"),
                2 => await Loader<Sprite>("JellyTileSpriteTwo"),
                3 => await Loader<Sprite>("JellyTileSpriteThree"),
                _ => null
            };
        }

        private async UniTask LoadTilesPrefab()
        {
            var key = gameData.CurrentLevel.LevelNumber;
            BackgroundTilePrefab = await Loader<GameObject>("BackgroundTilePrefab");
            TilePrefab = await Loader<GameObject>("TilePrefab");
            FXRemoveTilePrefab = await Loader<GameObject>("FXPrefab"+key);
        }
        
        private async UniTask LoadSprits()
        {
            BgTileSpriteCandy = await Loader<Sprite>("BackgroundTileSprite");
            // blank
            BlankTileSpriteOne = await Loader<Sprite>("BlankTileSpriteOne"); 
            BlankTileSpriteTwo = await Loader<Sprite>("BlankTileSpriteTwo"); 
            BlankTileSpriteThree = await Loader<Sprite>("BlankTileSpriteThree");
            // jelly
            JellyTileSpriteOne = await Loader<Sprite>("JellyTileSpriteOne");
            JellyTileSpriteTwo = await Loader<Sprite>("JellyTileSpriteTwo");
        }
    }
}