using System;
using UnityEngine;

namespace Game.Tiles
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour
    {
        public TileConfig TileConfig { get; private set; }
        public bool IsInteractable { get; private set; }
        public bool IsMatched { get; private set; }
        public TileKind tileKind { get; private set; }
        private Transform _jellyTransform;
        public Transform JellyTransform
        {
            get
            {
                if  (_jellyTransform != null)
                    return _jellyTransform;
                
                throw new Exception("Jelly transform is null");
            }
            set
            {
                if (_jellyTransform == null)
                {
                    _jellyTransform = value;
                    _jellyTransform.GetComponent<SpriteRenderer>().sortingLayerName = "ModifierForTiles";
                }
            }
        }
        
        public void SetTileConfig(TileConfig tileConfig)
        {
            TileConfig = tileConfig;
            IsInteractable = tileConfig.IsInteractable;
            IsMatched = false;
            GetComponent<SpriteRenderer>().sprite = tileConfig.Sprite; // для jelly не ставится спрайт
        }
        
        public void SetIsInteractable(bool value) => IsInteractable = value;
        
        public void SetMatch(bool value) => IsMatched = value;
        public void SetTileKind(TileKind value) => tileKind = value;
    }
}