using System;
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
        
        public async UniTask Reveal(GameObject target, float delay)
        {
            _cts = new CancellationTokenSource();
            target.transform.localScale = Vector3.zero * 0.1f;
            await target.transform.DOScale(Vector3.one, delay).SetEase(Ease.OutBounce);
            _cts.Cancel();
        }

        public async UniTask HideTile(GameObject target)
        {
            _cts = new CancellationTokenSource();
            await target.transform.DOScale(Vector3.zero, 0.08f)
                .SetEase(Ease.OutBounce).WithCancellation(_cts.Token);
            target.SetActive(false);
            target.transform.localScale = Vector3.one;
        }

        public void DoPunchAnimate(GameObject target, Vector3 scale, float duretion)
        {
            target.transform.DOPunchScale(scale, duretion, 1, 0.5f);
        }

        public void MoveUI(RectTransform target, Vector3 position, float duration, Ease ease)
        {
            target.DOAnchorPos(position, duration).SetEase(ease);
        }

        public void AnimateTile(Tile tile, float value)
        {
            tile.transform.DOScale(value, 0.3f).SetEase(Ease.OutCubic);
        }

        public void MoveTile(Tile tile, Vector3 position, Ease ease)
        {
             tile.transform.DOLocalMove(position, 0.2f).SetEase(ease);
        }
        
        public void MoveObject(GameObject go, Vector3 position, float duration, Ease ease)
        {
            go.transform.DOLocalMove(position, duration).SetEase(ease);
        }

        public void Dispose()
        {
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