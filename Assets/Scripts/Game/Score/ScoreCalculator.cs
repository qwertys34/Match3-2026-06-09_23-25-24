using System;
using Game.MatchTiles;
using Game.Tiles;

namespace Game.Score
{
    public class ScoreCalculator
    {
        private GameProgress _gameProgress;

        public ScoreCalculator(GameProgress gameProgress) => 
            _gameProgress = gameProgress;

        public void CalculateScoreToAdd(MatchDirection matchDirection)
        {
            switch (matchDirection)
            {
                case MatchDirection.Horizontal:
                case MatchDirection.Vertical:
                    _gameProgress.AddScore(50);
                    break;
                case MatchDirection.LongHorizontal:
                case MatchDirection.LongVertical:
                    _gameProgress.AddScore(100);
                    break;
                case MatchDirection.Multiply:
                    _gameProgress.AddScore(200);
                    break;
                case MatchDirection.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(matchDirection), matchDirection, null);
            }
        }

        public void CalculateAmountRemainingTiles(TileKind tileKind)
        {
            _gameProgress.AddOneFromTileRemoved(tileKind);
        }

        public int CalculateScore(MatchDirection matchDirection)
        {
            return matchDirection switch
            {
                MatchDirection.Horizontal or MatchDirection.Vertical => 50,
                MatchDirection.LongHorizontal or MatchDirection.LongVertical => 100,
                MatchDirection.Multiply => 200,
                _ => 0
            };
        }

        public int AddScoreForInteractabel(TileKind tileKind)
        {
            switch (tileKind)
            {
                case TileKind.Normal:
                case TileKind.Blank:
                case  TileKind.Jelly:
                case TileKind.RocketVertical:   
                case TileKind.RocketHorizontal:    
                    _gameProgress.AddScore(50);
                    return 50;
            }

            return 0;
        }
    }
}