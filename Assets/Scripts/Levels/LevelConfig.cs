using System.Collections.Generic;
using Game.Tiles;
using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {

        [Header("Grid")]
        public int width;
        public int height;
        public List<InteractableTile> interactableTilesLayout;
        
        [Header("Level")]
        [SerializeField] private int levelNumber;
        public int scoreForOneStar;
        public int scoreForTwoStar;
        public int scoreForThreeStar;
        public int moves;
        public int amountStartBlank;
        public int amountStartJelly;

        public int Width => width;
        public int Height => height;
        public List<InteractableTile> InteractableTilesLayout => interactableTilesLayout;
        
        public int LevelNumber => levelNumber;
        public int ScoreForOneStar => scoreForOneStar;
        public int ScoreForTwoStar => scoreForTwoStar;
        public int ScoreForThreeStar => scoreForThreeStar;
        public int Moves => moves;
    }
    
}
