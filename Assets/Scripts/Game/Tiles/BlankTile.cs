using ResurcesLoading;
using UnityEngine;

namespace Game.Tiles
{
    public class BlankTile : Tile
    {
        public int State { get; private set; }
        private void OnEnable()
        {
            State = 1;
        }

        public void ChangeState(GameResurcesLoader resurcesLoader, int state = 0)
        {
            if (state == 0) state = State += 1;

            if (state == 1)
            {
                GetComponent<SpriteRenderer>().sprite = resurcesLoader.BlankTileSpriteOne;
            }
            else if (state == 2)
                GetComponent<SpriteRenderer>().sprite = resurcesLoader.BlankTileSpriteTwo;
            else if (state == 3)
                GetComponent<SpriteRenderer>().sprite = resurcesLoader.BlankTileSpriteThree;
        }

        public bool CanAlive() => !(State > 3);
    }
}