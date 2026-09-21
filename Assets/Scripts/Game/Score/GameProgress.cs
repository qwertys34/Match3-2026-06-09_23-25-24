using System;
using Cysharp.Threading.Tasks;
using Game.Tiles;
using Input;
using Levels;
using UnityEngine;

namespace Game.Score
{
    public class GameProgress : IDisposable
    {
        public event Action OnGamePause;
        public event Func<UniTask> OnNewGoalToStar;
        public event Action OnMove;
        public event Func<TileKind, UniTask> AmountTilesChanged;
        public bool IsWin { get; private set; } = false;
        public bool IsPause { get; private set; }
        public int Score { get; private set; }
        public int GoalAmountScore { get; private set; }
        public int CurrentAmountStars { get; private set; }
        
        public int ScoreForOneStar { get; private set; }
        public int ScoreForTwoStar { get; private set; }
        public int ScoreForThreeStar { get; private set; }
        public int Moves { get; private set; }
        public int CurrentAmountBlank { get; private set; }
        public int CurrentAmountJelly { get; private set; }

        private InputReader _inputReader;
        
        public GameProgress(InputReader inputReader)
        {
            this._inputReader = inputReader;
            this._inputReader.Pause += GamePause;
            this._inputReader.UIClick += GamePauseWithMouse;
        }
        
        public void LoadLevelConfig(LevelConfig levelConfig)
        {
            Score = 0;
            CurrentAmountStars = 0;
            GoalAmountScore = levelConfig.ScoreForOneStar;
            ScoreForOneStar = levelConfig.ScoreForOneStar;
            ScoreForTwoStar = levelConfig.ScoreForTwoStar;
            ScoreForThreeStar = levelConfig.ScoreForThreeStar;
            Moves = levelConfig.Moves;
            CurrentAmountBlank = levelConfig.amountStartBlank;
            CurrentAmountJelly = levelConfig.amountStartJelly;
        }

        public void AddScore(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            Score += value;
            if (CheckScoreForStar())
                OnNewGoalToStar?.Invoke();
        }

        public void AddOneFromTileRemoved(TileKind tileKind)
        {
            switch (tileKind)
            {
                case TileKind.Blank:
                    CurrentAmountBlank-=1;
                    break;
                case  TileKind.Jelly:
                    CurrentAmountJelly-=1;
                    break;
            }

            AmountTilesChanged?.Invoke(tileKind);
        }

        public bool CheckGoalAmount()
        {
            if (CurrentAmountJelly == 0 && CurrentAmountBlank == 0)
            {
                IsWin = true;
                return true;
            }

            IsWin = false;
            return false; // false
        }

        public void SpendMoves()
        {
            Moves--;
            OnMove?.Invoke();
        }

        private void GamePause()
        {
            IsPause = !IsPause;
            OnGamePause?.Invoke();
        }

        private void GamePauseWithMouse()
        {
            if (IsPause)
            {
                IsPause = false;
                OnGamePause?.Invoke();
                Debug.Log("все норм");
            }
        }
        
        public void OnGamePauseWithMouse() => IsPause = !IsPause;
        
        private bool CheckScoreForStar()
        {
            if (Score >= GoalAmountScore && GoalAmountScore == ScoreForOneStar)
            {
                GoalAmountScore = ScoreForTwoStar;
                CurrentAmountStars = 1;
                return true;
            }
            else if (Score >= GoalAmountScore && GoalAmountScore == ScoreForTwoStar)
            {
                GoalAmountScore = ScoreForThreeStar;
                CurrentAmountStars = 2;
                return true;
            }
            else if (Score >= GoalAmountScore && GoalAmountScore == ScoreForThreeStar)
            {
                CurrentAmountStars = 3;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _inputReader.Pause -= GamePause;
            _inputReader.UIClick -= GamePauseWithMouse;
        }
    }
}
