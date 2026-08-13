using System;
using System.Collections.Generic;
using System.Threading;
using Animations;
using Audio;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Game.Score;
using Menu.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI
{
    public class EndGamePanelView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private RectTransform panelRectTransform;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private TMP_Text title;

        [SerializeField] private List<Image> starImageList;
        
        private IAnimation _animation;
        private AudioManager _audioManager;
        private GameProgress _gameProgress;
        private EndGame _endGame;
        private SetupLevelSequence _setupLevelSequence;

        private IAsyncSceneLoading _sceneLoading;
        
        private CancellationTokenSource _cts;
        private bool _isWinCondition;

        private readonly string _win = "You have won";
        private readonly string _lose = "You have loose";
        
        [Inject] public void Construct(IAnimation animation, AudioManager audioManager, EndGame endGame,
            GameProgress gameProgress, IAsyncSceneLoading sceneLoading, SetupLevelSequence setupLevelSequence)
        {
            _animation = animation;
            _audioManager = audioManager;
            _endGame = endGame;
            _gameProgress = gameProgress;
            _sceneLoading = sceneLoading;
            _setupLevelSequence = setupLevelSequence;
        }

        private async UniTask RevelStarsAnimate()
        {
            var count = _gameProgress.CurrentAmountStars;
            for (int i = 0; i < count; i++)
            {
                starImageList[i].gameObject.SetActive(true);
                await _animation.Reveal(starImageList[i].gameObject, 0.4f);
            }
        }

        private void OnEnable()
        {
            menuButton.onClick.AddListener(ExitGame);
            replayButton.onClick.AddListener(RestartLevel);
            nextLevelButton.onClick.AddListener(NextLevel);

            foreach (var starImage in starImageList) 
                starImage.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            menuButton.onClick.RemoveListener(ExitGame);
            replayButton.onClick.RemoveListener(RestartLevel);
            nextLevelButton.onClick.RemoveListener(NextLevel);
            
            foreach (var starImage in starImageList)
                starImage.gameObject.SetActive(false);
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void ExitGame() => _endGame.End(_isWinCondition, menuButton);

        private async void NextLevel()
        {
            if (_setupLevelSequence.isOneLevel) RestartLevel(); // если последний lvl(бесконечный),
            // перезагружаем
            
            if (_gameProgress.IsWin && !_sceneLoading.IsLastLevel(_setupLevelSequence))
            {
                await _sceneLoading.LoadNextScene(_audioManager, _setupLevelSequence);
            }
            else
            {
                nextLevelButton.interactable = false;
                nextLevelButton.enabled = false;
            }
        }

        private async void RestartLevel()
        {
            await _sceneLoading.RestartScene();
        }

        public async void ShowEndGamePanel(bool isWinCondition)
        {
            _isWinCondition = isWinCondition;
            title.text = isWinCondition ? _win : _lose;
            await StartAnimation();
            await RevelStarsAnimate();
            menuButton.interactable = true;
            replayButton.interactable = true;
            nextLevelButton.interactable = true;
        }
        
        private async UniTask StartAnimation()
        {
            _cts = new CancellationTokenSource();
            _audioManager.PlayWhoosh();
            panel.SetActive(true);
            if (panelRectTransform != null)
                _animation.MoveUI(panelRectTransform, new Vector3(1, -110, 1), 0.5f, Ease.InOutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), _cts.IsCancellationRequested);
            _audioManager.StopMusic();
            if (_isWinCondition)
                _audioManager.PlayWin();
            else 
                _audioManager.PlayLoose();
        }
    }
}