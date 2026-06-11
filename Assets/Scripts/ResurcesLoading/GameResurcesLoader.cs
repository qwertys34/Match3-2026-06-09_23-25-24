using Game.Tiles;
using UnityEngine;

namespace ResurcesLoading
{
    public class GameResurcesLoader : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private TileSetConfig tileSetConfig;

        public GameObject TilePrefab => tilePrefab;

        public TileSetConfig TileSetConfig => tileSetConfig;
    }
}