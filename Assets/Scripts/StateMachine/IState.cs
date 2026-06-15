using Cysharp.Threading.Tasks;

namespace StateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
}