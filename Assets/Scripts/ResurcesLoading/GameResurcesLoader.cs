using Game.Tiles;
using UnityEngine;

namespace ResurcesLoading
{
    public class GameResurcesLoader : MonoBehaviour
    {
        

        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject blankTilePrefab;
        [SerializeField] private TileSetConfig tileSetConfig;
        [SerializeField] private TileConfig blankConfig;
        [SerializeField] private GameObject backgroundTilePrefab;
        [SerializeField] private GameObject fxRemoveTilePrefab;

        [SerializeField] private Sprite whiteBackgroundTileSprite;
        [SerializeField] private Sprite blackBackgroundTileSprite;
        
        public GameObject FXRemoveTilePrefab => fxRemoveTilePrefab;
        
        public GameObject BlankTilePrefab => blankTilePrefab;

        public TileConfig BlankConfig => blankConfig;

        public GameObject TilePrefab => tilePrefab;

        public TileSetConfig TileSetConfig => tileSetConfig;
        
        public GameObject BackgroundTilePrefab => backgroundTilePrefab;

        public Sprite WhiteBackgroundTileSprite => whiteBackgroundTileSprite;

        public Sprite BlackBackgroundTileSprite => blackBackgroundTileSprite;
    }
}