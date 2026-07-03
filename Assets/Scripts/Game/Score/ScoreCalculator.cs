using Game.MatchTiles;
using UnityEngine;

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
                    _gameProgress.AddScore(50);
                    Debug.Log("+50");
                    break;
                case MatchDirection.Vertical:
                    _gameProgress.AddScore(50);
                    Debug.Log("+50");
                    break;
                case MatchDirection.LongHorizontal:
                    _gameProgress.AddScore(100);
                    Debug.Log("+100");
                    break;
                case MatchDirection.LongVertical:
                    _gameProgress.AddScore(100);
                    Debug.Log("+100");
                    break;
                case MatchDirection.Multiply:
                    _gameProgress.AddScore(200);
                    Debug.Log("+200");
                    break;
            }
        }
    }
}