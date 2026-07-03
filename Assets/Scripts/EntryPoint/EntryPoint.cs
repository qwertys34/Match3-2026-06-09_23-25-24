using Animations;
using Game.Board;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using UnityEngine;
using VContainer;
using Grid = Game.GridSystem.Grid;

namespace EntryPoint
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameBoard gameBoard;
        private StateMachine.StateMachine _stateMachine;
        private Grid _grid;
        private IAnimation _animation;
        private MatchFinder _matchFinder;
        private TilePool _tilePool;
        private ScoreCalculator _scoreCalculator;
        private GameProgress _gameProgress;

        private void Start()
        {
            _stateMachine = new StateMachine.StateMachine(gameBoard, _grid, _animation, _matchFinder, _tilePool,
                _gameProgress, _scoreCalculator);
            _gameProgress.LoadLevelConfig(gameBoard.LevelConfig.GoalScore, gameBoard.LevelConfig.Moves);
        }

        [Inject]
        private void Construct(Grid grid, IAnimation anim, MatchFinder matchFinder, TilePool tilePool,
            ScoreCalculator scoreCalculator, GameProgress gameProgress)
        {
            _grid = grid;
            _animation = anim;
            _matchFinder = matchFinder;
            _tilePool = tilePool;
            _scoreCalculator = scoreCalculator;
            _gameProgress = gameProgress;
        }
    }
}