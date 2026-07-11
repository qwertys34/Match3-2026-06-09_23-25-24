using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Game.Tiles;
using Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResurcesLoading
{
    public class GameResurcesLoader 
    {
        public GameObject TilePrefab { get; private set; }
        public TileConfig BlankConfig { get; private set; }
        public GameObject BackgroundTilePrefab { get; private set; }
        public GameObject FXRemoveTilePrefab { get; private set; }
        public Sprite LitghBgTileSprite { get; private set; }
        public Sprite DarkBgTileSprite { get; private set; }
        
        public List<TileConfig> CurrentTileSet { get; private set; }
        
        private GameData _gameData;
        private CancellationTokenSource _cts;

        public GameResurcesLoader(GameData gameData) => _gameData = gameData;

        public async UniTask Load()
        {
            CurrentTileSet = new List<TileConfig>();
            
            if (_gameData.CurrentLevel.LevelType == LevelType.Kingdom)
                await LoadSet("Kingdom");
            else if (_gameData.CurrentLevel.LevelType == LevelType.Gem) 
                await LoadSet("Gem");

            await LoadTilesPrefab();
            await LoadBlankTile();
            await LoadBackgroundSprits();
        }

        private async UniTask LoadSet(string key)
        {
            _cts = new CancellationTokenSource();
            var set = Addressables.LoadAssetAsync<TileSetConfig>(key);
            await set.ToUniTask();
            if (set.Status == AsyncOperationStatus.Succeeded)
            {
                CurrentTileSet = set.Result.Set;
                Addressables.Release(set);
            }
            _cts.Cancel();
        }

        private async UniTask LoadTilesPrefab()
        {
            _cts = new CancellationTokenSource();
            var BGTile = Addressables
                .LoadAssetAsync<GameObject>("BackgroundTilePrefab");
            var tilePrefab = Addressables
                .LoadAssetAsync<GameObject>("TilePrefab");
            var FXPrefab = Addressables
                .LoadAssetAsync<GameObject>("FXPrefab");

            await BGTile.ToUniTask();
            await tilePrefab.ToUniTask();
            await FXPrefab.ToUniTask();

            if (BGTile.Status == AsyncOperationStatus.Succeeded 
                && tilePrefab.Status == AsyncOperationStatus.Succeeded 
                && FXPrefab.Status == AsyncOperationStatus.Succeeded)
            {
                BackgroundTilePrefab = BGTile.Result;
                TilePrefab = tilePrefab.Result;
                FXRemoveTilePrefab = FXPrefab.Result;
                
                Addressables.Release(BGTile);
                Addressables.Release(tilePrefab);
                Addressables.Release(FXPrefab);
            }
            
            _cts.Cancel();
        }
        
        private async UniTask LoadBlankTile()
        {
            _cts = new CancellationTokenSource();
            var blank = Addressables.LoadAssetAsync<TileConfig>("BlankTile");
            await blank.ToUniTask();
            if (blank.Status == AsyncOperationStatus.Succeeded)
            {
                BlankConfig = blank.Result;
                Addressables.Release(blank);
            }
            _cts.Cancel();
        }
        
        private async UniTask LoadBackgroundSprits()
        {
            _cts = new CancellationTokenSource();
            var darkBG = Addressables.LoadAssetAsync<Sprite>("Dark");
            var lightBG = Addressables.LoadAssetAsync<Sprite>("Light");
            
            await darkBG.ToUniTask();
            await lightBG.ToUniTask();
            
            if (darkBG.Status == AsyncOperationStatus.Succeeded
                && lightBG.Status == AsyncOperationStatus.Succeeded)
            {
                DarkBgTileSprite = darkBG.Result;
                LitghBgTileSprite = lightBG.Result;
                Addressables.Release(darkBG);
                Addressables.Release(lightBG);
            }
            _cts.Cancel();
        }
    }
}