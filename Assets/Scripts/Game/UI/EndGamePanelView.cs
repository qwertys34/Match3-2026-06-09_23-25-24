using System;
using System.Threading;
using Animations;
using Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Score;
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
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text title;
        
        private IAnimation _animation;
        private AudioManager _audioManager;
        private EndGame _endGame;
        private CancellationTokenSource _cts;
        private bool _isWinCondition;

        private readonly string _win = "You have won";
        private readonly string _loose = "You have loose";
        
        [Inject] public void Construct(IAnimation animation, AudioManager audioManager, EndGame endGame)
        {
            _animation = animation;
            _audioManager = audioManager;
            _endGame = endGame;
        }

        private void OnEnable() => closeButton.onClick.AddListener(ExitGame);

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(ExitGame);
            _cts?.Dispose();
        }

        private void ExitGame() => _endGame.End(_isWinCondition);

        public async void ShowEndGamePanel(bool isWinCondition)
        {
            _isWinCondition = isWinCondition;
            title.text = isWinCondition ? _win : _loose;
            await StartAnimation();
            closeButton.interactable = true;
        }
        
        private async UniTask StartAnimation()
        {
            _cts = new CancellationTokenSource();
            _audioManager.PlayWhoosh();
            panel.SetActive(true);
            _animation.MoveUI(panelRectTransform, new Vector3(1, -110, 1), 0.5f, Ease.InOutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), _cts.IsCancellationRequested);
            _audioManager.StopMusic();
            if (_isWinCondition)
                _audioManager.PlayWin();
            else 
                _audioManager.PlayLoose();
            await UniTask.Delay(TimeSpan.FromSeconds(1f), _cts.IsCancellationRequested);
        }
    }
}