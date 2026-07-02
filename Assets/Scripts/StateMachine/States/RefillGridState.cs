using System;
using UnityEngine;

namespace StateMachine.States
{
    public class RefillGridState : IState, IDisposable
    {
        private IStateSwitcher _stateSwitcher;

        public RefillGridState(IStateSwitcher stateSwitcher)
        {
            _stateSwitcher = stateSwitcher;
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public void Enter()
        {
            Debug.Log("Refill grid state entered");
            _stateSwitcher.SwitchState<PlayerTurnState>();
        }

        public void Exit()
        {
            
        }
    }
}