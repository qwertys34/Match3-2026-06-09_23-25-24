using Levels;

namespace Game.Tiles
{
    public class InteractablesTilesSetup
    {
        public TileKind[,]  tileKind {get; private set;}

        public void SetupBlanks(LevelConfig levelConfig)
        {
            tileKind = new TileKind[levelConfig.Width, levelConfig.Height];
            for (int i = 0; i < levelConfig.InteractableTilesLayout.Count; i++)
            {
                tileKind[levelConfig.InteractableTilesLayout[i].XPos, levelConfig.InteractableTilesLayout[i].YPos] 
                    = levelConfig.InteractableTilesLayout[i].TileKind;
            }
        }
    }
}