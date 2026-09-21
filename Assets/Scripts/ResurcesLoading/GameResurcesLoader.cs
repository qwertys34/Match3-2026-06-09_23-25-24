using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Game.Tiles;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = System.Random;

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
        
        // superCandy
        public TileConfig SuperCandyConfig  { get; private set; }
        
        private readonly GameData gameData;
        private CancellationTokenSource _cts;
        
        // Список для хранения всех handle
        private readonly List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();
        
        public GameResurcesLoader(GameData gameData) => this.gameData = gameData;
        
        public event Func<UniTask> LoadComplete;

        public void Dispose()
        {
            _cts?.Dispose();
            ReleaseAllHandles();
        }
        
        public async UniTask Load()
        {
            await LoadSet();
            await LoadTilesPrefab();
            await LoadSprits();
            
            if (LoadComplete != null)
                await LoadComplete.Invoke();
        }

        private async UniTask LoadSet()
        {
            CurrentTileSet = new List<TileConfig>();
            
            var tileSetConfig = await Loader<TileSetConfig>("Candy");
            if (tileSetConfig != null)
                CurrentTileSet = tileSetConfig.Set;
            
            BlankConfig = await Loader<TileConfig>("BlankConfig");
            VerticalRocketConfig = await Loader<TileConfig>("VerticalRocketConfig");
            HorizontalRocketConfig = await Loader<TileConfig>("HorizontalRocketConfig");
            JellyConfig = await Loader<TileConfig>("JellyConfig");
            BombConfig = await Loader<TileConfig>("BombConfig");
            SuperCandyConfig = await Loader<TileConfig>("SuperCandyConfig");
        }

        private async UniTask<T> Loader<T>(string key)
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
            await asyncOperationHandle.ToUniTask();
            
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                // Сохраняем handle для последующего освобождения
                _handles.Add(asyncOperationHandle);
                return asyncOperationHandle.Result;
            }
            
            Debug.LogError($"Failed to load asset: {key}");
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
            //var key = gameData.CurrentLevel.LevelNumber;
            var key = UnityEngine.Random.Range(1, 8);
            BackgroundTilePrefab = await Loader<GameObject>("BackgroundTilePrefab");
            TilePrefab = await Loader<GameObject>("TilePrefab");

            string targetKey = "FXPrefab" + key;
            string fallbackKey = "FXPrefab1";
    
            bool exists = await AssetExists(targetKey);

            if (exists)
                FXRemoveTilePrefab = await Loader<GameObject>(targetKey);
            else
                FXRemoveTilePrefab = await Loader<GameObject>(fallbackKey);
        }
        
        private async UniTask<bool> AssetExists(string key)
        {
            try
            {
                var handle = Addressables.LoadResourceLocationsAsync(key);
                await handle.ToUniTask();
        
                bool exists = handle.Status == AsyncOperationStatus.Succeeded 
                              && handle.Result != null 
                              && handle.Result.Count > 0;
        
                // Сохраняем и этот handle
                _handles.Add(handle);
                return exists;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при проверке ассета {key}: {ex.Message}");
                return false;
            }
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
        
        private void ReleaseAllHandles()
        {
            foreach (var handle in _handles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _handles.Clear();
        }
    }
}