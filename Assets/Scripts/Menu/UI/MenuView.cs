using System;
using System.Collections.Generic;
using System.Threading;
using Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        
        [Inject] private void Configure(AnimationManager animationManager) => 
            _animation = animationManager;

        public async UniTask StartAnimation()
        {
            _cts = new CancellationTokenSource();
            
            /*_animation.MoveUI(leftTower, new Vector3(231f, 405f, 0), 0.3f, Ease.InOutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), _cts.IsCancellationRequested); 
            
            _animation.MoveUI(rightTower, new Vector3(-231f, 405f, 0), 0.2f, Ease.InOutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), _cts.IsCancellationRequested); 
            
            _animation.MoveUI(middleWall, new Vector3(0, 245f, 0), 0.3f, Ease.InOutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), _cts.IsCancellationRequested); 
            
            _animation.MoveUI(logo, new Vector3(-92, -40, 0), 0.3f, Ease.OutBounce);
            await UniTask.Delay(TimeSpan.FromSeconds(0.6f), _cts.IsCancellationRequested);*/
            
            foreach (var button in levelButtons)
            {
                button.SetActive(true);
                await _animation.Reveal(button, 0.2f);
            }
            
            _cts.Cancel();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}