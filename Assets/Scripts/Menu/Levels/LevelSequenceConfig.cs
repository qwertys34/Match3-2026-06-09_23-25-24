using System.Collections.Generic;
using Levels;
using UnityEngine;

namespace Menu.Levels
{
    [CreateAssetMenu(fileName = "LevelSequenceConfig", menuName = "Configs/LevelSequenceConfig")]
    public class LevelSequenceConfig : ScriptableObject
    {
        [SerializeField] private List<LevelConfig> levelConfigs;
        public List<LevelConfig> LevelConfigs => levelConfigs;    
        

        /*private void OnValidate()
        {
            if (levelConfigs.Count != 5)
                Debug.LogWarning("amount level configs must be 5");
        }*/
    }
}
