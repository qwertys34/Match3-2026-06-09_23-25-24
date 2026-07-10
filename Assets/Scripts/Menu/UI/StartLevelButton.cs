using Cysharp.Threading.Tasks;
using Data;
using Menu.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Menu.UI
{
    public class StartLevelButton : MonoBehaviour
    {
        [SerializeField] private Button lable;
        [SerializeField] private TMP_Text text;
        
        public int Number { get; private set; }
        private StartGame _startGame;
        private SetupLevelSequence _setupLevelSequence;

        private void OnEnable()
        {
            lable.onClick.AddListener(StartLevelButtonClick);
        }
        
        private void OnDisable()
        {
            lable.onClick.RemoveListener(StartLevelButtonClick);
        }

        [Inject] private void Construct(SetupLevelSequence setupLevelSequence, StartGame startGame)
        {
            _setupLevelSequence = setupLevelSequence;
            _startGame = startGame;
        }

        private async void StartLevelButtonClick()
        {
            if (_setupLevelSequence.CurrentLevelSequence.LevelConfigs[Number - 1].LevelNumber <= 5)
            {
                await _startGame.Start(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[Number - 1]);
            }
            else 
                await _startGame.Start(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[Number - 6]);
        }
        
        public void SetNumber(int value) => Number = Mathf.Clamp(value, 1, 10);
        
        public void SetLabel() => text.text = Number.ToString();
        
        public void SetButtonInteractable(bool value) => lable.interactable = value;
    }
}