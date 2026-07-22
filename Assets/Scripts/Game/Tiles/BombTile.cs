using System;
using System.Collections.Generic;
using Animations;
using Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Board;
using Game.MatchTiles;
using Game.Score;
using Game.Utils;
using ResurcesLoading;
using UnityEngine;
using Grid = Game.GridSystem.Grid;

namespace Game.Tiles
{
    public class BombTile : Tile, IDisposable
    {
        private Sequence _bombSequence;
        private Sequence _explodeSequence;
        private bool _isExploded = false;
        public async UniTask Explode(Grid grid, GameResurcesLoader resurcesLoader,
            IAnimation animation, ScoreCalculator scoreCalculator, FXPool fxPool,
            GameBoard gameBoard, AudioManager audioManager, MatchFinder matchFinder)
        {
            List<Tile> tilesToExplode = new();
            var bombTile = grid.GetValue(transform.position);
            var posBombTile = grid.WorldToGrid(transform.position);
            var x = posBombTile.x;
            var y = posBombTile.y;
            
            tilesToExplode.Add(bombTile);
            tilesToExplode.Add(grid.GetValue(x - 1, y + 1));
            tilesToExplode.Add(grid.GetValue(x - 1, y));
            tilesToExplode.Add(grid.GetValue(x - 1, y - 1));
            tilesToExplode.Add(grid.GetValue(x, y + 1));
            tilesToExplode.Add(grid.GetValue(x, y - 1));
            tilesToExplode.Add(grid.GetValue(x + 1, y + 1));
            tilesToExplode.Add(grid.GetValue(x + 1, y));
            tilesToExplode.Add(grid.GetValue(x + 1, y - 1));

            foreach (var tile in tilesToExplode)
            {
                if (tile != null)
                {
                    switch (tile.tileKind)
                    {
                        case TileKind.Bomb:
                            await AnimateBomb(tile.gameObject);
                            tile.gameObject.SetActive(false);
                            //grid.SetValue(tile.transform.position, null); пока просто уничтожаю, не создавая новые по игре
                            Destroy(tile.gameObject);
                            _isExploded = true;
                            break;
                        case TileKind.Jelly:
                            var jellyTile = (JellyTile)tile;
                            audioManager.PlayRemove();
                            var amountScore = scoreCalculator.AddScoreForInteractabel(TileKind.Jelly);
                            fxPool.GetFX(jellyTile.transform.position, gameBoard.transform, amountScore);
                            await animation.ShakeAnimate(jellyTile.transform, 0.1f, Ease.InQuint);
                            jellyTile.ChangeState(jellyTile.JellyTransform, resurcesLoader);
                            if (jellyTile.IsSimpleTile())
                            {
                                scoreCalculator.CalculateAmountRemainingTiles(TileKind.Jelly);
                                continue;
                            }
                            if (!jellyTile.CanAlive())
                            {
                                await animation.HideTile(jellyTile.gameObject);
                                jellyTile.gameObject.SetActive(false);
                                grid.SetValue(jellyTile.transform.position, null);
                            }
                            break;
                        case TileKind.Blank:
                            var blankTile = (BlankTile)tile;
                            audioManager.PlayRemove();
                            var amScore = scoreCalculator.AddScoreForInteractabel(TileKind.Jelly);
                            fxPool.GetFX(blankTile.transform.position, gameBoard.transform, amScore);
                            await animation.ShakeAnimate(blankTile.transform, 0.1f, Ease.InQuint);
                            if (!blankTile.CanAlive())
                            {
                                await animation.HideTile(blankTile.gameObject);
                                blankTile.gameObject.SetActive(false);
                                scoreCalculator.CalculateAmountRemainingTiles(TileKind.Jelly);
                                continue;
                            }
                            blankTile.ChangeState(resurcesLoader);
                            break;
                        default:
                            audioManager.PlayRemove();
                            //await AnimateExplode(tile.gameObject, animation);
                            await UniTask.WaitUntil(() => _isExploded); // ждем пока не пройдет анимация
                            // взрыва бомбы
                            await AnimateExplode(tile.gameObject);
                            //await animation.HideTile(tile.gameObject);
                            var amntScore = scoreCalculator.CalculateScore(matchFinder.CurrentMatchResult.MatchDirection);
                            fxPool.GetFX(tile.transform.position, gameBoard.transform, amntScore);
                            tile.gameObject.SetActive(false);
                            grid.SetValue(tile.transform.position, null);
                            break;
                    }
                }
            }
            
        }
        
        private async UniTask AnimateBomb(GameObject obj)
        {
            var nextColor = Color.black;
            var sr = obj.GetComponent<SpriteRenderer>();

            _bombSequence = DOTween.Sequence()
                // Быстрая тряска с нарастанием
                .Append(obj.transform.DOShakePosition(0.4f, strength: 0.3f, vibrato: 30))
                .SetEase(Ease.InCubic)
                // Мгновенное сжатие перед взрывом
                .Append(obj.transform.DOScale(Vector3.one * 0.3f, 0.15f))
                .SetEase(Ease.InBack)
                // Взрывное расширение
                .Append(obj.transform.DOScale(Vector3.one * 1.5f, 0.3f))
                .SetEase(Ease.OutElastic)
                .Join(sr.DOColor(nextColor, 0.2f))
                .SetEase(Ease.InQuint);
    
            await _bombSequence.Play();
        }
        
        private async UniTask AnimateExplode(GameObject obj)
        {
            var nextColor = Color.black; //new Color(98f, 98f, 98f, 0f);
            var sr = obj.GetComponent<SpriteRenderer>();
            
            _explodeSequence = DOTween.Sequence(sr.DOColor(nextColor, 0.4f)).SetEase(Ease.InExpo);
            await _explodeSequence.Play();
        }

        public void Dispose()
        {
            _bombSequence.Kill();
            _explodeSequence.Kill();
            _bombSequence =  null;
            _explodeSequence = null;
        }
    }
}