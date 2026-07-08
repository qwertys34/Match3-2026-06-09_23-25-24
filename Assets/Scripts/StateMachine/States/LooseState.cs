
using Game.UI;

namespace StateMachine.States
{
    public class LooseState : IState
    {
        private EndGamePanelView _panel;

        public LooseState(EndGamePanelView panel)
        {
            _panel = panel;
        }

        public void Enter()
        {
            _panel.ShowEndGamePanel(false);
        }

        public void Exit()
        {
            
        }
    }
}