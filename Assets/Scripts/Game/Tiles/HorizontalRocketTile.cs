using System;
using System.Collections.Generic;
using System.Threading;
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
    public class HorizontalRocketTile : Tile
    {
        public CancellationTokenSource shakeCts { get; set; }

        public async UniTask Run(Grid grid, IAnimation animation, ScoreCalculator scoreCalculator,
            FXPool fxPool, MatchFinder matchFinder, GameBoard gameBoard, AudioManager audioManager,
            GameResurcesLoader gameResurcesLoader)
        {
            var tilePos = grid.WorldToGrid(transform.position);
            
            // Убираем текущий тайл с его позиции
            grid.SetValue(tilePos.x, tilePos.y, null);
            
            // Создаем два снаряда: один летит вправо, другой влево
            var rightProjectile = CreateProjectile();
            var leftProjectile = CreateProjectile();
            
            // Устанавливаем начальные позиции
            rightProjectile.transform.position = transform.position;
            leftProjectile.transform.position = transform.position;
            
            // Сначала скрываем оригинальный тайл
            await animation.HideTile(gameObject);
            
            // Запускаем движение вправо и влево параллельно
            var rightTask = MoveProjectile(rightProjectile, grid, animation, scoreCalculator, fxPool, 
                audioManager, gameBoard, gameResurcesLoader, tilePos, 1, matchFinder); // +1 для движения вправо
            var leftTask = MoveProjectile(leftProjectile, grid, animation, scoreCalculator, fxPool, 
                audioManager, gameBoard, gameResurcesLoader, tilePos, -1, matchFinder); // -1 для движения влево
            
            await UniTask.WhenAll(rightTask, leftTask);
            
            // Уничтожаем оригинальный тайл только после завершения всех операций
            if (this != null && gameObject != null)
            {
                await animation.HideTile(gameObject);
            }
        }
        
        private GameObject CreateProjectile()
        {
            // Создаем простой спрайт вместо копирования всего gameObject с компонентом RocketTile
            var projectile = new GameObject("RocketProjectile");
            projectile.transform.SetParent(transform.parent);
            
            // Копируем спрайт
            var spriteRenderer = projectile.AddComponent<SpriteRenderer>();
            var originalRenderer = GetComponent<SpriteRenderer>();
            if (originalRenderer != null)
            {
                spriteRenderer.sprite = originalRenderer.sprite;
                spriteRenderer.sortingOrder = originalRenderer.sortingOrder;
                spriteRenderer.sortingLayerID = originalRenderer.sortingLayerID;
                spriteRenderer.sortingLayerName = originalRenderer.sortingLayerName;
            }
            
            // Копируем масштаб
            projectile.transform.localScale = transform.localScale;
            
            return projectile;
        }
        
        private async UniTask MoveProjectile(GameObject projectile, Grid grid, IAnimation animation, 
            ScoreCalculator scoreCalculator, FXPool fxPool, AudioManager audioManager, 
            GameBoard gameBoard, GameResurcesLoader gameResurcesLoader, 
            Vector2Int startPos, int direction, MatchFinder matchFinder)
        {
            var currentPos = startPos;
            List<VerticalRocketTile> activatedRockets = new(); 
            List<BombTile> activatedBomb = new(); 
            
            for (int i = 0; i < grid.Width; i++)
            {
                var nextTile = new Vector2Int(currentPos.x + direction, currentPos.y);
                
                if (!grid.IsValidPosition(nextTile.x, nextTile.y))
                    break;
                
                var nextTileValue = grid.GetValue(nextTile.x, nextTile.y);
                
                // Если следующая клетка пустая, просто двигаем снаряд дальше
                if (nextTileValue == null)
                {
                    audioManager.PlayRemove();
                    await animation.MoveObject(projectile, grid.GridToWorld(nextTile.x, nextTile.y),
                        0.1f, Ease.InQuart);
                    
                    currentPos.x += direction;
                    continue;
                }
                
                if (nextTileValue.tileKind == TileKind.Blank)
                {
                    var blankTile = (BlankTile)nextTileValue;
                    
                    // Ломаем BlankTile на одно состояние
                    blankTile.ChangeState(gameResurcesLoader);
                    
                    // Эффекты и очки за разрушение состояния BlankTile
                    audioManager.PlayRemove();
                    var amountScore = scoreCalculator.AddScoreForInteractabel(TileKind.Blank);
                    fxPool.GetFX(blankTile.transform.position, gameBoard.transform, amountScore);
                    
                    // Проверяем, уничтожен ли BlankTile полностью
                    if (!blankTile.CanAlive())
                    {
                        await animation.HideTile(blankTile.gameObject);
                        // BlankTile полностью разрушен, убираем его с сетки
                        grid.SetValue(nextTile.x, nextTile.y, null);
                        scoreCalculator.CalculateAmountRemainingTiles(TileKind.Blank);
                        // Двигаем снаряд на место бывшего BlankTile
                        await animation.MoveObject(projectile, grid.GridToWorld(nextTile.x, nextTile.y),
                            0.1f, Ease.InQuart);
                        
                        currentPos.x += direction;
                        continue;
                    }
                    else
                    {
                        // BlankTile еще существует, уничтожаем снаряд
                        await animation.HideTile(projectile);
                        return;
                    }
                }
                else if (nextTileValue.tileKind == TileKind.Jelly)
                {
                    var jellyTile = (JellyTile)nextTileValue;
                    
                    // Ломаем JellyTile на одно состояние
                    jellyTile.ChangeState(jellyTile.JellyTransform, gameResurcesLoader);
                    await animation.ShakeAnimate(jellyTile.transform, 0.1f, Ease.InQuint);
                    
                    // Эффекты и очки за разрушение состояния JellyTile
                    audioManager.PlayRemove();
                    var amountScore = scoreCalculator.AddScoreForInteractabel(TileKind.Jelly);
                    fxPool.GetFX(jellyTile.transform.position, gameBoard.transform, amountScore);
                    
                    if (jellyTile.IsSimpleTile())
                        scoreCalculator.CalculateAmountRemainingTiles(TileKind.Jelly);
                    
                    // Проверяем, уничтожен ли JellyTile полностью
                    if (!jellyTile.CanAlive())
                    {
                        // JellyTile полностью разрушен, убираем его с сетки
                        await animation.HideTile(jellyTile.gameObject);
                        grid.SetValue(nextTile.x, nextTile.y, null);
                        
                        // Двигаем снаряд на место бывшего JellyTile
                        await animation.MoveObject(projectile, grid.GridToWorld(nextTile.x, nextTile.y),
                            0.1f, Ease.InQuart);
                        
                        currentPos.x += direction;
                        continue;
                    }
                    else
                    {
                        // JellyTile еще существует, уничтожаем снаряд
                        await animation.HideTile(projectile);
                        return;
                    }
                }
                else if (nextTileValue.tileKind == TileKind.RocketHorizontal)
                {
                    audioManager.PlayRemove();
                    await animation.HideTile(nextTileValue.gameObject);
                    var amountScore = scoreCalculator.AddScoreForInteractabel(TileKind.RocketHorizontal);
                    fxPool.GetFX(nextTileValue.transform.position, gameBoard.transform, amountScore);
                    
                    grid.SetValue(nextTile.x, nextTile.y, null);
                    await animation.MoveObject(projectile, grid.GridToWorld(nextTile.x, nextTile.y),
                        0.1f, Ease.InQuart);
                    currentPos.x += direction;
                    continue;
                }
                else if (nextTileValue.tileKind == TileKind.RocketVertical)
                {
                    var verticalRocket = (VerticalRocketTile)nextTileValue;
                    
                    // Добавляем в список активированных
                    activatedRockets.Add(verticalRocket);
                    
                    // Запускаем анимацию тряски (не ждем завершения)
                    var shakeCts = new CancellationTokenSource();
                    _ = animation.ShakeAnimateUntil(verticalRocket.transform, Ease.InQuint, shakeCts);
                    
                    verticalRocket.shakeCts = shakeCts;
                    
                    currentPos.x += direction;
                    continue;
                }
                // Бомба - пропускаем
                else if (nextTileValue.tileKind == TileKind.Bomb)
                {
                    var bomb = (BombTile)nextTileValue;
                    
                    activatedBomb.Add(bomb);
                    
                    var shakeCts = new CancellationTokenSource();
                    _ = animation.ShakeAnimateUntil(bomb.transform, Ease.InQuint, shakeCts);
                    
                    bomb.shakeCts = shakeCts;
                    
                    currentPos.x += direction;
                    continue;
                }
                
                // Обычный тайл - уничтожаем его и двигаем снаряд на его место
                audioManager.PlayRemove();
                if (nextTileValue != null)
                {
                    await animation.HideTile(nextTileValue.gameObject);
                    var amountScore = scoreCalculator.AddScoreForInteractabel(TileKind.RocketHorizontal);
                    fxPool.GetFX(nextTileValue.transform.position, gameBoard.transform, amountScore);
                }
                
                grid.SetValue(nextTile.x, nextTile.y, null);
                
                await animation.MoveObject(projectile, grid.GridToWorld(nextTile.x, nextTile.y),
                    0.1f, Ease.InQuart);
                
                currentPos.x += direction;
            }
            
            // Уничтожаем снаряд в конце пути
            if (projectile != null && projectile.TryGetComponent(out SpriteRenderer sr))
            {
                await animation.HideTile(projectile);
            }
            
            // В конце полета запускаем все активированные вертикальные ракеты
            foreach (var rocket in activatedRockets)
            {
                if (rocket == null || rocket.gameObject == null) continue;
                
                // Останавливаем тряску
                rocket.shakeCts?.Cancel();
                rocket.shakeCts?.Dispose();
                
                // Убираем ракету с сетки перед запуском
                var rocketPos = grid.WorldToGrid(rocket.transform.position);
                grid.SetValue(rocketPos.x, rocketPos.y, null);
                
                // Запускаем ракету
                await rocket.Run(grid, animation, scoreCalculator, fxPool,
                    matchFinder, gameBoard, audioManager, gameResurcesLoader);
            }
            
            foreach (var bomb in activatedBomb)
            {
                if (bomb == null || bomb.gameObject == null) continue;
                
                // Останавливаем тряску
                bomb.shakeCts?.Cancel();
                bomb.shakeCts?.Dispose();
                
                // Запускаем ракету
                await bomb.Explode(grid, gameResurcesLoader, animation, scoreCalculator, fxPool, gameBoard,
                    audioManager, matchFinder);
            }
        }
    }
}