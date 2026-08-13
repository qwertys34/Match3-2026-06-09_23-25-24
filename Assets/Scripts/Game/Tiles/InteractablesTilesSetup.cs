using Levels;

namespace Game.Tiles
{
    public class InteractablesTilesSetup
    {
        public TileKind[,]  tileKind {get; private set;}

        public void SetupInteractables(LevelConfig levelConfig)
        {
            tileKind = new TileKind[levelConfig.Width, levelConfig.Height];
            for (int i = 0; i < levelConfig.InteractableTilesLayout.Count; i++)
            {
                tileKind[levelConfig.InteractableTilesLayout[i].xPos, levelConfig.InteractableTilesLayout[i].yPos] 
                    = levelConfig.InteractableTilesLayout[i].tileKind;
            }
        }
    }
}