using System.Collections.Generic;
using System.Threading;
using Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Input;
using UnityEngine;
using VContainer;

namespace Menu.UI
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField] private RectTransform leftTower;
        [SerializeField] private RectTransform rightTower;
        [SerializeField] private RectTransform middleWall;
        [SerializeField] private RectTransform logo;
        [SerializeField] private List<GameObject> levelButtons = new();

        private AnimationManager _animation;
        private CancellationTokenSource _cts;
        private bool _currentOrientation;
        
        [Inject] private void Configure(AnimationManager animationManager) => 
            _animation = animationManager;

        public async UniTask StartAnimation()
        {
            // Отменяем предыдущую анимацию, если она еще идет
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            _currentOrientation = InputReader.IsVectical();
            
            float logoYPosition = _currentOrientation ? -481.7f : -392.8f;
            _animation.MoveUI(logo, new Vector3(0, logoYPosition, 0), 0.6f, Ease.OutBounce);
            
            bool isLandscape = Screen.width > Screen.height;
            Vector3 targetScale = isLandscape ? Vector3.one : Vector3.one * 0.86f;
            
            foreach (var button in levelButtons)
            {
                await _animation.RevealUI(button, 0.2f, targetScale);
            }
        }

        private void Update()
        {
            bool newOrientation = InputReader.IsVectical();
            if (_currentOrientation != newOrientation)
            {
                _currentOrientation = newOrientation;
                
                // Используем константы
                float newY = newOrientation ? -478f : -392.8f;
                
                // Плавно перемещаем или сразу устанавливаем позицию
                logo.anchoredPosition = new Vector2(logo.anchoredPosition.x, newY);
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}