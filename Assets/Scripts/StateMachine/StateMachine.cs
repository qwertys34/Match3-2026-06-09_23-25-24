using System.Collections.Generic;
using System.Linq;
using Game.Board;
using StateMachine.States;

namespace StateMachine
{
    public class StateMachine : IStateSwitcher
    {
        private List<IState> _states;
        private IState _currentState;
        private GameBoard _gameBoard;

        public StateMachine(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;
            _states = new List<IState>() {
                new PrepareState(this, _gameBoard)
            };
            
            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : IState
        {
            var newState = _states.FirstOrDefault(state => state is T);
            _currentState.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }
    }
}