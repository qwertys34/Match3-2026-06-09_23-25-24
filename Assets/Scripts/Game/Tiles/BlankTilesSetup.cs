using Levels;

namespace Game.Tiles
{
    public class BlankTilesSetup
    {
        public bool [,] Blanks { get; private set; }

        public void SetupBlanks(LevelConfig levelConfig)
        {
            Blanks = new bool[levelConfig.Width, levelConfig.Height];
            for (int i = 0; i < levelConfig.BlankTilesLayout.Count; i++)
            {
                Blanks[levelConfig.BlankTilesLayout[i].XPos, levelConfig.BlankTilesLayout[i].YPos] = true;
            }
        }
    }
}