using System;
using System.Threading;
using Animations;
using Cysharp.Threading.Tasks;
using ResurcesLoading;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Game.Tiles
{
    public class BackgroundTilesSetup : IDisposable
    {
        private readonly GameResurcesLoader _gameResurcesLoader;
        private IAnimation _animation;
        private IObjectResolver _objectResolver;
        private CancellationTokenSource _cts;
        private BlankTilesSetup _blankTilesSetup;
        
        public BackgroundTilesSetup(GameResurcesLoader gameResurcesLoader, IAnimation animation,
            IObjectResolver objectResolver, BlankTilesSetup blankTilesSetup)
        {
            _gameResurcesLoader = gameResurcesLoader;
            _animation = animation;
            _objectResolver = objectResolver;
            _blankTilesSetup = blankTilesSetup;
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _objectResolver?.Dispose();
        }

        public async UniTask SetupBackgroundTiles(int width, int height, Transform parent)
        {
            _cts = new CancellationTokenSource();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_blankTilesSetup.Blanks[x, y]) continue;
                    var backgroundTile = CreateBackgroundTile(x, y, parent);
                    var delay = Random.Range(0.8f, 1.5f);
                    _animation.Reveal(backgroundTile, delay);
                }
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), _cts.IsCancellationRequested);
            _cts.Cancel();
        }

        private GameObject CreateBackgroundTile(int x, int y, Transform parent)
        {
            var backgroundTile = _objectResolver.Instantiate(_gameResurcesLoader.BackgroundTilePrefab,
                new Vector3(x, y, 0.1f), Quaternion.identity, parent);

            if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
                backgroundTile.GetComponent<SpriteRenderer>().sprite = _gameResurcesLoader.BlackBackgroundTileSprite;
            else 
                backgroundTile.GetComponent<SpriteRenderer>().sprite = _gameResurcesLoader.WhiteBackgroundTileSprite;
            return backgroundTile;
        }
    }
}