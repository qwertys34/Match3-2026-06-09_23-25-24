using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Tiles;
using Levels;
using Random = UnityEngine.Random;

namespace Game.Utils
{
    public class GeneratorLevelConfig
    {
        private readonly TileKind[] interactableTiles =
        {
            TileKind.RocketVertical,
            TileKind.RocketHorizontal,
            TileKind.Bomb,
            TileKind.SuperCandy
        };
        
        public async UniTask<LevelConfig> GenerateLevelConfig(LevelConfig levelConfig)
        {
            var lenX = 7/*Random.Range(5, 8)*/;         
            var lenY = 7/*Random.Range(5, 8)*/;
            var newTilesList = new InteractableTile[lenX, lenY];
            
            for (int x = 0; x < lenX; x++)
            {
                for (int y = 0; y < lenY; y++)
                {
                    newTilesList[x, y] = new InteractableTile();
                    newTilesList[x, y].xPos = x;
                    newTilesList[x, y].yPos = y;
                    
                    if (CheckTileValidation(newTilesList,x + 1, y, lenX, lenY) &&
                        CheckTileValidation(newTilesList,x + 2, y, lenX, lenY) &&
                        CheckTileValidation(newTilesList,x - 1, y, lenX, lenY) &&
                        CheckTileValidation(newTilesList,x - 2, y, lenX, lenY) &&
                        CheckTileValidation(newTilesList, x, y + 1, lenX, lenY) &&
                        CheckTileValidation(newTilesList, x, y + 2, lenX, lenY) &&
                        CheckTileValidation(newTilesList, x, y - 1, lenX, lenY) &&
                        CheckTileValidation(newTilesList, x, y - 2, lenX, lenY))
                    {
                        newTilesList[x, y].tileKind = interactableTiles[Random.Range(0, interactableTiles.Length)];
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }
            }
            WriteInfo(levelConfig, newTilesList, lenX, lenY);
            return levelConfig;
        }

        private bool CheckTileValidation(InteractableTile[,] newTilesList, int checkX, int checkY, int lenX, int lenY)
        {
            if (checkX < 0 || checkX >= lenX || checkY < 0 || checkY >= lenY)
                return true;
    
            if (newTilesList[checkX, checkY] == null)
                return true;
    
            return !interactableTiles.Contains(newTilesList[checkX, checkY].tileKind);
        }

        private void WriteInfo(LevelConfig levelConfig, InteractableTile[,] newInteractableTilesLayout, int lenX, int lenY)
        {
            levelConfig.width = lenX;
            levelConfig.height = lenY;
    
            var tilesList = new List<InteractableTile>();
            for (int x = 0; x < lenX; x++)
            {
                for (int y = 0; y < lenY; y++)
                {
                    tilesList.Add(newInteractableTilesLayout[x, y]);
                }
            }
            
            levelConfig.interactableTilesLayout = tilesList;
        }
    }
}