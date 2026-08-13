using System;
using UnityEngine;

namespace Game.Tiles
{
    [Serializable]
    public class InteractableTile 
    {
        public int xPos;
        public int yPos;
        public TileKind tileKind;
        
        /*public int XPos => xPos;
        public int YPos => yPos;
        public TileKind TileKind => tileKind;*/
    }
}