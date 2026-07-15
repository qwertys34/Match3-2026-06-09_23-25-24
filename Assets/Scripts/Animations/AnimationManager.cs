using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Tiles;
using UnityEngine;

namespace Animations
{
    public class AnimationManager : IAnimation, IDisposable
    {
        private CancellationTokenSource _cts;
        private List<Tween> _activeTweens = new();
        
        public async UniTask Reveal(GameObject target, float delay)
        {
            _cts = new CancellationTokenSource();
            target.transform.localScale = Vector3.zero * 0.1f;
            var tween = target.transform.DOScale(Vector3.one, delay).SetEase(Ease.OutBounce);
            _activeTweens.Add(tween);
            await tween;
            _cts.Cancel();
        }

        public async UniTask HideTile(GameObject target)
        {
            _cts = new CancellationTokenSource();
            var tween = target.transform.DOScale(Vector3.zero, 0.08f)
                .SetEase(Ease.OutBounce);
            _activeTweens.Add(tween);
            await tween.WithCancellation(_cts.Token);
            target.SetActive(false);
            target.transform.localScale = Vector3.one;
        }

        public async UniTask DoPunchAnimate(GameObject target, Vector3 scale, float duretion)
        {
            var tween = target.transform.DOPunchScale(scale, duretion, 1, 0.5f);
            _activeTweens.Add(tween);
            await tween;
        }

        public void MoveUI(RectTransform target, Vector3 position, float duration, Ease ease)
        {
            var tween = target.DOAnchorPos(position, duration).SetEase(ease);
            _activeTweens.Add(tween);
        }

        public async UniTask AnimateTile(Tile tile, float value)
        {
            var tween = tile.transform.DOScale(value, 0.3f).SetEase(Ease.OutCubic);
            _activeTweens.Add(tween);
            await tween;
        }

        public void MoveTile(Tile tile, Vector3 position, Ease ease)
        {
             var tween = tile.transform.DOLocalMove(position, 0.2f).SetEase(ease);
             _activeTweens.Add(tween);
        }
        
        public async UniTask MoveObject(GameObject go, Vector3 position, float duration, Ease ease)
        {
            var tween = go.transform.DOLocalMove(position, duration).SetEase(ease);
            _activeTweens.Add(tween);
            await tween;
        }

        public void Dispose()
        {
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive())
                    tween.Kill();
            }
            _activeTweens.Clear();
            _cts?.Cancel();
            _cts?.Dispose();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetAllStatics()
        {
            DOTween.Clear();
        }
    }
}