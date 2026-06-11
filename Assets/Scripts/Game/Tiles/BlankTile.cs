using System;
using UnityEngine;

namespace Game.Tiles
{
    [Serializable]
    public class BlankTile
    {
        [SerializeField] private int xPos;
        [SerializeField] private int yPos;

        public int XPos => xPos;
        public int YPos => yPos;
    }
}