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
        public GameObject TilePrefab { get; private set; }
        public TileConfig BlankConfig { get; private set; }
        public GameObject BackgroundTilePrefab { get; private set; }
        public GameObject FXRemoveTilePrefab { get; private set; }
        public Sprite BgTileSpriteCandy { get; private set; }
        public Sprite BlankTileSpriteOne { get; private set; }
        public Sprite BlankTileSpriteTwo { get; private set; }
        public Sprite BlankTileSpriteThree { get; private set; }
        public List<TileConfig> CurrentTileSet { get; private set; }
        
        
        
        
        public readonly GameData gameData;
        private CancellationTokenSource _cts;
        
        public GameResurcesLoader(GameData gameData) => this.gameData = gameData;

        public void Dispose() => _cts?.Dispose();
        
        public async UniTask Load()
        {
            await LoadSet();
            await LoadTilesPrefab();
            await LoadSprits();
        }

        private async UniTask LoadSet()
        {
            CurrentTileSet = new List<TileConfig>();
            
            var key = gameData.CurrentLevel.LevelType;
            CurrentTileSet = (await Loader<TileSetConfig>(key.ToString())).Set;
            
            BlankConfig = await Loader<TileConfig>("BlankTile");
            
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
            BlankTileSpriteOne = await Loader<Sprite>("BlankTileSpriteOne"); 
            BlankTileSpriteTwo = await Loader<Sprite>("BlankTileSpriteTwo"); 
            BlankTileSpriteThree = await Loader<Sprite>("BlankTileSpriteThree"); 
        }//

    }
}