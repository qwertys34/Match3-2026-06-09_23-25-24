using System.Collections.Generic;
using Game.Tiles;

namespace Game.MatchTiles
{
    public class MatchResult
    {
        private List<Tile> _conectedTiles;
        private MatchDirection _matchDirection;

        public MatchResult(List<Tile> conectedTiles, MatchDirection matchDirection)
        {
            _conectedTiles = conectedTiles;
            _matchDirection = matchDirection;
        }

        public List<Tile> ConectedTiles => _conectedTiles;

        public MatchDirection MatchDirection => _matchDirection;
        
        
    }
}