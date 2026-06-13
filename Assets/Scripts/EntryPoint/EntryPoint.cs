using System;
using Game.Board;
using UnityEngine;

namespace EntryPoint
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameBoard gameBoard;
        private StateMachine.StateMachine _stateMachine;

        private void Start()
        {
            _stateMachine = new StateMachine.StateMachine(gameBoard);
        }
    }
}