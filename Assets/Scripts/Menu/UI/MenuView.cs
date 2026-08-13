using System;
using System.Collections;
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
        private bool current;
        private bool newOrientation;
        
        [Inject] private void Configure(AnimationManager animationManager) => 
            _animation = animationManager;

        public async UniTask StartAnimation()
        {
            _cts = new CancellationTokenSource();
            
            current = InputReader.IsVectical();
            if (current)
                _animation.MoveUI(logo, new Vector3(0, -481.7f, 0), 0.6f, Ease.OutBounce);
            else 
                _animation.MoveUI(logo, new Vector3(0, -392.8f, 0), 0.6f, Ease.OutBounce);
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.6f), _cts.IsCancellationRequested);
            
            bool isLandscape = Screen.width > Screen.height;
            Vector3 targetScale = isLandscape ? Vector3.one : Vector3.one * 0.86f;
            
            foreach (var button in levelButtons)
            {
                await _animation.RevealUI(button, 0.2f, targetScale);
            }
            
            _cts.Cancel();
        }

        private void Update()
        {
            newOrientation = InputReader.IsVectical();
            if (current != newOrientation)
            {
                current = newOrientation;
                var newPos = Vector3.zero;
                newPos.y = newOrientation ? -478f : -392.8f; 
                logo.anchoredPosition = newPos;
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}