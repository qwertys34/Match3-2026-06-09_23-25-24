using UnityEngine;

namespace StateMachine.States
{
    public class WinState : IState
    {
        public void Enter()
        {
            Debug.Log("YOU WIN)))");
        }

        public void Exit()
        {
            
        }
    }
}