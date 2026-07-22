using ResurcesLoading;
using UnityEngine;

namespace Game.Tiles
{
    public class JellyTile : Tile
    {
        public int State { get; private set; }
        
        private void OnEnable()
        {
            State = 1;
        }
        
        public void ChangeState(Transform pos, GameResurcesLoader resurcesLoader, int state = 0)
        {
            if (state == 0) state = State += 1;

            if (state == 1)
            {
                pos.GetComponent<SpriteRenderer>().sprite = resurcesLoader.JellyTileSpriteOne;
            }
            else if (state == 2)
                pos.GetComponent<SpriteRenderer>().sprite = resurcesLoader.JellyTileSpriteTwo;
            else if (state == 3)
                pos.GetComponent<SpriteRenderer>().sprite = null;
        }

        public bool CanAlive() => !(State > 3);
        
        public bool IsSimpleTile() => State == 3;
    }
}