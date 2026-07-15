using System;
using UnityEngine;

namespace Game.Tiles
{
    [Serializable]
    public class InteractableTile 
    {
        [SerializeField] private int xPos;
        [SerializeField] private int yPos;
        [SerializeField] private TileKind tileKind;
        
        public int XPos => xPos;
        public int YPos => yPos;
        public TileKind TileKind => tileKind;
    }
}