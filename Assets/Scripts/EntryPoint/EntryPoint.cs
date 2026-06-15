using Animations;
using Game.Board;
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

        private void Start()
        {
            _stateMachine = new StateMachine.StateMachine(gameBoard, _grid, _animation);
        }

        [Inject]
        private void Construct(Grid grid, IAnimation anim)
        {
            _grid = grid;
            _animation = anim;
        }
    }
}