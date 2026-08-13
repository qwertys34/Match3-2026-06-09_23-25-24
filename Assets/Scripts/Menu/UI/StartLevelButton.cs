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

        private void StartLevelButtonClick()
        {
            // Запускаем асинхронную операцию и забываем (fire-and-forget)
            StartLevelAsync().Forget();
        }

        private async UniTaskVoid StartLevelAsync()
        {
            var length = _setupLevelSequence.CurrentLevelSequence.LevelConfigs.Count;
            Debug.Log("last level number: " + _setupLevelSequence.CurrentLevelSequence.LevelConfigs[length-1].LevelNumber);
    
            if (_setupLevelSequence.CurrentLevelSequence.LevelConfigs[length-1].LevelNumber == 5)
            {
                await _startGame.Start(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[Number - 1]);
            }
            else if (_setupLevelSequence.CurrentLevelSequence.LevelConfigs[length - 1].LevelNumber == 10)
                await _startGame.Start(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[Number - 6]);
            else
                await _startGame.StartRandomLevel(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[0]);
        }
        
        public void SetNumber(int value) => Number = value/*Mathf.Clamp(value, 1, 10)*/;
        
        public void SetLabel() => text.text = Number.ToString();
        public void SetLabel(string t) => text.text = t;
        
        public void SetButtonInteractable(bool value) => lable.interactable = value;
    }
}