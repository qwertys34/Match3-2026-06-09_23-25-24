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
using YG;

namespace Game.UI
{
    public class EndGamePanelView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] public RectTransform panelRectTransform;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private TMP_Text title;
        [SerializeField] private Button pauseButton;        

        [SerializeField] private List<Image> starImageList;

        private GameData _gameData;
        private IAnimation _animation;
        private AudioManager _audioManager;
        public GameProgress _gameProgress;
        private EndGame _endGame;
        private SetupLevelSequence _setupLevelSequence;
        
        private IAsyncSceneLoading _sceneLoading;
        
        private CancellationTokenSource _cts;
        private bool _isWinCondition;

        private readonly string _win = "You have won";
        private readonly string _lose = "You have loose";

        private bool _hasBeenAdv = false;
        
        [Inject] public void Construct(IAnimation animation, AudioManager audioManager, EndGame endGame,
            GameProgress gameProgress, IAsyncSceneLoading sceneLoading, SetupLevelSequence setupLevelSequence,
            GameData gameData)
        {
            _animation = animation;
            _audioManager = audioManager;
            _endGame = endGame;
            _gameProgress = gameProgress;
            _sceneLoading = sceneLoading;
            _setupLevelSequence = setupLevelSequence;
            _gameData = gameData;
            
            _gameProgress.OnGamePause += OnGamePause;
        }

        private async UniTask RevelStarsAnimate()
        {
            var count = _gameProgress.CurrentAmountStars;
            for (int i = 0; i < count; i++)
            {
                starImageList[i].gameObject.SetActive(true);
                if (i == 1) // звезда по середине
                    await _animation.Reveal(starImageList[i].gameObject, 0.4f, 1.18f); 
                else
                    await _animation.Reveal(starImageList[i].gameObject, 0.4f);
            }
        }

        public void OnGamePause()
        {
            Debug.Log($"Game pause status: {_gameProgress.IsPause}");
            if (_gameProgress.IsPause)
                ShowPausePanel().Forget();
            else 
                HidePausePanel().Forget();
        }
        
        public void OnGamePauseWithMouse()
        {
            Debug.Log($"Game pause status: {_gameProgress.IsPause}");
            if (_gameProgress.IsPause)
                ShowPausePanel().Forget();
            else 
                HidePausePanel().Forget();
        }
        
        private void Awake()
        {
            pauseButton.onClick.AddListener(() =>
            {
                _gameProgress.OnGamePauseWithMouse();
                OnGamePauseWithMouse();
            });
            menuButton.onClick.AddListener(ExitGame);
            replayButton.onClick.AddListener(RestartLevel);
            nextLevelButton.onClick.AddListener(NextLevel);

            foreach (var starImage in starImageList) 
                starImage.gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            pauseButton.onClick.RemoveAllListeners();
            menuButton.onClick.RemoveListener(ExitGame);
            replayButton.onClick.RemoveListener(RestartLevel);
            nextLevelButton.onClick.RemoveListener(NextLevel);
            
            foreach (var starImage in starImageList)
                starImage.gameObject.SetActive(false);
            _cts?.Cancel();
            _cts?.Dispose();
            _gameProgress.OnGamePause -= OnGamePause;
        }

        private void ExitGame() => _endGame.End(_isWinCondition, menuButton);

        private async void NextLevel()
        {
            Debug.Log("СЛЕДУЮЩИЙ УРОВЕНЬ");
            if (!_hasBeenAdv)
            {
                Debug.Log("ПРОДОЛЖЕНИЕ");
                YG2.InterstitialAdvShow();
                _hasBeenAdv = true;
            }
            if (_setupLevelSequence.isOneLevel) RestartLevel(); // если последний lvl(бесконечный),
            // перезагружаем
            
            if (_gameProgress.IsWin && !_sceneLoading.IsLastLevel(_setupLevelSequence))
            {
                await _sceneLoading.LoadNewNextScene(_audioManager, _setupLevelSequence);
            }
            else if (_gameData.CurrentLevelIndex > _gameData.CurrentLevel.LevelNumber)
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
            if (!_hasBeenAdv)
            {
                YG2.InterstitialAdvShow();
                _hasBeenAdv = false;
            }
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
        private async UniTaskVoid ShowPausePanel()
        {
            gameObject.SetActive(true);
            title.text = null;
            await StartPauseAnimation(true);
            await RevelStarsAnimate();
            menuButton.interactable = true;
            replayButton.interactable = true;
            nextLevelButton.interactable = true;
        }

        private async UniTaskVoid HidePausePanel()
        {
            await StartPauseAnimation(false);
            menuButton.interactable = false;
            replayButton.interactable = false;
            nextLevelButton.interactable = false;
            gameObject.SetActive(false);
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

        private async UniTask StartPauseAnimation(bool isPause)
        {
            _cts = new CancellationTokenSource();
            _audioManager.PlayWhoosh();
            if (isPause)
            {
                panel.SetActive(true);
                if (panelRectTransform != null)
                    await _animation.MoveUIWithCanceletion(panelRectTransform, new Vector3(1, -110, 1), 0.5f,
                        Ease.InOutBack, _cts);
            }
            else
            {
                try
                {
                    await _animation.HideTileUI(gameObject, 0.5f);

                    var count = _gameProgress.CurrentAmountStars;
                    for (var i = 0; i < count; i++) 
                        starImageList[i].gameObject.SetActive(false);
                }
                catch
                {
                    Debug.Log("Ошибка");
                }
                panel.SetActive(false);
            }
        }
    }
}