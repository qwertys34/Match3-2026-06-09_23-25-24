using System.Collections.Generic;
using System.Linq;
using Animations;
using Game.Board;
using Game.GridSystem;
using StateMachine.States;

namespace StateMachine
{
    public class StateMachine : IStateSwitcher
    {
        private List<IState> _states;
        private IState _currentState;
        private GameBoard _gameBoard;
        private Grid _grid;
        private IAnimation _animation;
        
        public StateMachine(GameBoard gameBoard,  Grid grid,  IAnimation animation)
        {
            _gameBoard = gameBoard;
            _grid = grid;
            _animation = animation;
            _states = new List<IState>() {
                new PrepareState(this, _gameBoard),
                new PlayerTurnState(this, _animation, _grid), 
                new SwapTilesState(this, _grid, _animation),
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