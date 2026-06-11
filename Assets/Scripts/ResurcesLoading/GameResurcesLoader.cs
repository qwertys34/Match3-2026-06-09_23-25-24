using Game.Tiles;
using UnityEngine;

namespace ResurcesLoading
{
    public class GameResurcesLoader : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject blanTilePrefab;
        [SerializeField] private TileSetConfig tileSetConfig;
        [SerializeField] private TileConfig blankConfig;
        
        public GameObject BlankTilePrefab => blanTilePrefab;

        public TileConfig BlankConfig => blankConfig;

        public GameObject TilePrefab => tilePrefab;

        public TileSetConfig TileSetConfig => tileSetConfig;
    }
}