using Game.UI;

namespace StateMachine.States
{
    public class WinState : IState
    {
        private EndGamePanelView _panel;

        public WinState(EndGamePanelView panel)
        {
            _panel = panel;
        }

        public void Enter()
        {
            _panel.ShowEndGamePanel(true);
        }

        public void Exit()
        {
            
        }
    }
}