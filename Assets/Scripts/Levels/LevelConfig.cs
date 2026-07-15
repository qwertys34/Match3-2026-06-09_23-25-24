using System.Collections.Generic;
using Game.Tiles;
using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Grid")]
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private List<InteractableTile> interactableTilesLayout;
        
        [Header("Level")]
        [SerializeField] private int levelNumber;
        [SerializeField] private LevelType levelType;
        [SerializeField] private int goalScore;
        [SerializeField] private int moves;

        public int Width => width;
        public int Height => height;
        public List<InteractableTile> InteractableTilesLayout => interactableTilesLayout;

        public int LevelNumber => levelNumber;
        public LevelType LevelType => levelType;
        public int GoalScore => goalScore;
        public int Moves => moves;
    }

    public enum LevelType
    {
        Kingdom,
        Gem,
        Candy
    }
}