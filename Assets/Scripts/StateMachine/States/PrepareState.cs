using Game.Board;
using Game.Tiles;
using UnityEngine;
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

        public async void Enter()
        {
            await _backgroundTilesSetup.SetupBackgroundTiles(_grid.Width, _grid.Height, _gameBoard.transform);
            _gameBoard.CreateBoard();
            _stateMachine.SwitchState<PlayerTurnState>();
        }

        public void Exit()
        {
            Debug.Log("Board was created!");
        }
    }
}