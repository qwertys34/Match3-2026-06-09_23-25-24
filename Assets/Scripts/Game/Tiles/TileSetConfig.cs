using System.Collections.Generic;
using UnityEngine;

namespace Game.Tiles
{
    [CreateAssetMenu(fileName = "TileSet", menuName = "Configs/TileSetConfig")]
    public class TileSetConfig : ScriptableObject
    {
        [SerializeField] private List<TileConfig> tiles;
        
        public List<TileConfig> Set => tiles;
        
    }
}