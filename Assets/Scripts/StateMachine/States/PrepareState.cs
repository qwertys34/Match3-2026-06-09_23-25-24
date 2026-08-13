using Cysharp.Threading.Tasks;
using Game.Board;
using Game.Tiles;
using Grid = Game.GridSystem.Grid;

namespace StateMachine.States
{
    public class PrepareState : IState
    {
        private IStateSwitcher _stateMachine;
        private GameBoard _gameBoard;
        private BackgroundTilesSetup _backgroundTilesSetup;
        private Grid _grid;
        
        public PrepareState(IStateSwitcher stateSwitcher, GameBoard gameBoard,
            BackgroundTilesSetup backgroundTilesSetup, Grid grid)
        {
            _stateMachine = stateSwitcher;
            _gameBoard = gameBoard;
            _backgroundTilesSetup = backgroundTilesSetup;
            _grid = grid;
        }

        public void Enter()
        {
            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            await _backgroundTilesSetup.SetupBackgroundTiles(_grid.Width, _grid.Height, _gameBoard.transform);
            await _gameBoard.CreateBoard();
            _stateMachine.SwitchState<PlayerTurnState>();
        }
        
        public void Exit()
        {
            
        }
    }
}