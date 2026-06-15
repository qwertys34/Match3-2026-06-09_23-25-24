using Cysharp.Threading.Tasks;
using Game.Board;
using UnityEngine;

namespace StateMachine.States
{
    public class PrepareState : IState
    {
        private IStateSwitcher _stateMachine;
        private GameBoard _gameBoard;
        
        public PrepareState(IStateSwitcher stateSwitcher, GameBoard gameBoard)
        {
            _stateMachine = stateSwitcher;
            _gameBoard = gameBoard;
        }

        public void Enter()
        {
            _gameBoard.CreateBoard();
            _stateMachine.SwitchState<PlayerTurnState>();
        }

        public void Exit()
        {
            Debug.Log("Board was created!");
        }
    }
}