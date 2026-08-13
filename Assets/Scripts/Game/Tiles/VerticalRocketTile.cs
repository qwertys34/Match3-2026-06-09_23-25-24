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
    public class VerticalRocketTile : Tile
    {
        public async UniTask Run(Grid grid, IAnimation animation, ScoreCalculator scoreCalculator,
            FXPool fxPool, MatchFinder matchFinder, GameBoard gameBoard, AudioManager audioManager,
            GameResurcesLoader gameResurcesLoader, List<Tile> horizontalRocketTilesToRemove)
        {
            var tilePos = grid.WorldToGrid(transform.position);
            
            // Убираем текущий тайл с его позиции
            grid.SetValue(tilePos.x, tilePos.y, null);
            
            // Создаем два снаряда: один летит вверх, другой вниз
            var upProjectile = CreateProjectile();
            var downProjectile = CreateProjectile();
            
            // Устанавливаем начальные позиции
            upProjectile.transform.position = transform.position;
            downProjectile.transform.position = transform.position;
            
            // Сначала скрываем оригинальный тайл
            await animation.HideTile(gameObject);
            
            // Запускаем движение вверх и вниз параллельно
            var upTask = MoveProjectile(upProjectile, grid, animation, scoreCalculator, fxPool, 
                audioManager, gameBoard, gameResurcesLoader, tilePos, 1, horizontalRocketTilesToRemove); // +1 для движения вверх
            var downTask = MoveProjectile(downProjectile, grid, animation, scoreCalculator, fxPool, 
                audioManager, gameBoard, gameResurcesLoader, tilePos, -1, horizontalRocketTilesToRemove); // -1 для движения вниз
            
            await UniTask.WhenAll(upTask, downTask);
            
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
            /*var originalRenderer = GetComponent<SpriteRenderer>();
            if (originalRenderer != null)
            {
                spriteRenderer.sprite = originalRenderer.sprite;
                spriteRenderer.sortingOrder = originalRenderer.sortingOrder;
                spriteRenderer.sortingLayerID = originalRenderer.sortingLayerID;
                spriteRenderer.sortingLayerName = originalRenderer.sortingLayerName;
            }*/
            
            // Копируем масштаб
            projectile.transform.localScale = transform.localScale;
            
            return projectile;
        }
        
        private async UniTask MoveProjectile(GameObject projectile, Grid grid, IAnimation animation, 
            ScoreCalculator scoreCalculator, FXPool fxPool, AudioManager audioManager, 
            GameBoard gameBoard, GameResurcesLoader gameResurcesLoader, 
            Vector2Int startPos, int direction, List<Tile> horizontalRocketTilesToRemove)
        {
            var currentPos = startPos;
            
            for (int i = 0; i < grid.Height; i++)
            {
                var nextTile = new Vector2Int(currentPos.x, currentPos.y + direction);
                
                if (!grid.IsValidPosition(nextTile.x, nextTile.y))
                    break;
                
                var nextTileValue = grid.GetValue(nextTile.x, nextTile.y);
                
                // Если следующая клетка пустая, просто двигаем снаряд дальше
                if (nextTileValue == null)
                {
                    audioManager.PlayRemove();
                    await animation.MoveObject(projectile, grid.GridToWorld(nextTile.x, nextTile.y),
                        0.1f, Ease.InQuart);
                    
                    currentPos.y += direction;
                    continue;
                }
                
                // Если это BlankTile
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
                        
                        currentPos.y += direction;
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
                        
                        currentPos.y += direction;
                        continue;
                    }
                    else
                    {
                        // JellyTile еще существует, уничтожаем снаряд
                        await animation.HideTile(projectile);
                        return;
                    }
                }
                /*else if (nextTileValue.tileKind == TileKind.RocketHorizontal)
                    horizontalRocketTilesToRemove.Remove(nextTileValue);*/
                
                // Обычный тайл - уничтожаем его и двигаем снаряд на его место
                audioManager.PlayRemove();
                if (nextTileValue != null)
                {
                    await animation.HideTile(nextTileValue.gameObject);
                    var amountScore = scoreCalculator.AddScoreForInteractabel(TileKind.RocketVertical);
                    fxPool.GetFX(nextTileValue.transform.position, gameBoard.transform, amountScore);
                }
                
                grid.SetValue(nextTile.x, nextTile.y, null);
                
                await animation.MoveObject(projectile, grid.GridToWorld(nextTile.x, nextTile.y),
                    0.1f, Ease.InQuart);
                
                currentPos.y += direction;
            }
            
            // Уничтожаем снаряд в конце пути, так как он не должен оставаться на поле
            if (projectile != null && projectile.TryGetComponent(out SpriteRenderer sr))
            {
                await animation.HideTile(projectile);
            }
        }
    }
}