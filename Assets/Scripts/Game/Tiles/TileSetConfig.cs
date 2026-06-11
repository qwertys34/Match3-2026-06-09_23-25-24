using System.Collections.Generic;
using UnityEngine;

namespace Game.Tiles
{
    [CreateAssetMenu(fileName = "TileSet", menuName = "TileSetConfig/TileSet")]
    public class TileSetConfig : ScriptableObject
    {
        [SerializeField] private List<TileConfig> tiles;
        
        public List<TileConfig> Set => tiles;
        
    }
}