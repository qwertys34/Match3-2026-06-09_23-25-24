using UnityEngine;

namespace StateMachine.States
{
    public class LooseState : IState
    {
        public void Enter()
        {
            Debug.Log("YOU LOOSE(((");
        }

        public void Exit()
        {
            
        }
    }
}