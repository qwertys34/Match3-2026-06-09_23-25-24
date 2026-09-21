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
        private readonly List<Tween> _activeTweens = new();
        private readonly List<Sequence> _activeSequences = new();
        
        public async UniTask Reveal(GameObject target, float delay, float size = 1f)
        {
            if (target == null) return;
            
            target.transform.localScale = Vector3.zero * 0.1f;
            var tween = target.transform.DOScale(Vector3.one*size, delay).SetEase(Ease.OutBounce);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }
        
        public async UniTask RevealUI(GameObject target, float delay, Vector3 scale)
        {
            if (target == null) return;
            
            target.SetActive(false);
            target.transform.localScale = Vector3.zero * 0.1f;
            target.SetActive(true);
            var sequence = DOTween.Sequence();
            var tween1 = target.transform.DOScale(Vector3.one * 1.5f, delay).SetEase(Ease.OutBounce);
            var tween2 = target.transform.DOScale(scale, delay).SetEase(Ease.OutBounce);
            
            _ = sequence.Append(tween1);
            _ = sequence.Join(tween2);
            
            _activeSequences.Add(sequence);
            
            try
            {
                await sequence.ToUniTask();
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }

        public async UniTask HideTile(GameObject target)
        {
            if (target == null) return;
            
            var tween = target.transform.DOScale(Vector3.zero, 0.08f)
                .SetEase(Ease.OutBounce);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
                if (target != null)
                {
                    target.SetActive(false);
                    target.transform.localScale = Vector3.one;
                    var spriteRenderer = target.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                        spriteRenderer.color = Color.white;
                }
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }
        
        public async UniTask HideTileUI(GameObject target, float duration = 0.08f)
        {
            if (target == null) return;
            
            var tween = target.transform.DOScale(Vector3.zero, duration)
                .SetEase(Ease.OutBounce);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
                if (target != null)
                {
                    target.SetActive(false);
                    target.transform.localScale = Vector3.one;
                }
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }

        public async UniTask DoPunchAnimate(GameObject target, Vector3 scale, float duration)
        {
            if (target == null) return;
            
            var tween = target.transform.DOPunchScale(scale, duration, 1, 0.5f);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }

        public void MoveUI(RectTransform target, Vector3 position, float duration, Ease ease)
        {
            if (target == null) return;
            
            var tween = target.DOAnchorPos(position, duration).SetEase(ease);
            _activeTweens.Add(tween);
        }
        public async UniTask MoveUIWithCanceletion(RectTransform target, Vector3 position, float duration, Ease ease,
            CancellationTokenSource cts)
        {
            if (target == null || cts == null) return;
            
            var tween = target.DOAnchorPos(position, duration).SetEase(ease);
            _activeTweens.Add(tween);
            await tween.WithCancellation(cts.Token);
        }

        public async UniTask AnimateTile(Tile tile, float value, float duration = 0.3f)
        {
            if (tile == null) return;
            
            var tween = tile.transform.DOScale(value, duration).SetEase(Ease.OutCubic);
            _activeTweens.Add(tween);
            
            await tween.ToUniTask();
        }

        public async UniTask AnimateSuperCandyMatch(Transform target, SpriteRenderer sr,
            float value, float duration)
        {
            if (target == null || sr == null) return;
            
            Material whiteMaterial = new Material(Shader.Find("UI/Default"));
            whiteMaterial.color = Color.white;
            var mainMaterial = sr.material;
            var mainColor = mainMaterial.color;
            
            sr.material = whiteMaterial;

            var sequence = DOTween.Sequence();
            _ = sequence.Append(target.DOScale(value, duration).SetEase(Ease.OutCubic));
            _ = sequence.Join(sr.DOColor(Color.white, duration).SetEase(Ease.Flash));
            _ = sequence.Join(target.DOLocalRotate(target.transform.rotation.eulerAngles + new Vector3(0, 0, 25), duration).SetEase(Ease.Flash));
            
            _activeSequences.Add(sequence);
            
            await sequence.Play().ToUniTask();
        }

        public async UniTask ShakeAnimate(Transform target, float duration, Ease ease)
        {
            if (target == null) return;
            
            var tween = target.DOShakePosition(duration, 0.3f).SetEase(ease);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }
        
        public async UniTask ShakeAnimateUntil(Transform target, Ease ease, CancellationTokenSource cts)
        {
            if (target == null) return;
            
            var tween = target.DOShakePosition(0.5f, strength:0.2f, vibrato: 40).SetEase(ease);
            await tween.WithCancellation(cts.Token);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Ошибка");
            }
        }

        public void MoveTile(Tile tile, Vector3 position, Ease ease)
        {
            if (tile == null) return;
            
            var tween = tile.transform.DOLocalMove(position, 0.2f).SetEase(ease);
            _activeTweens.Add(tween);
        }
        
        public async UniTask MoveObject(GameObject go, Vector3 position, float duration, Ease ease)
        {
            if (go == null) return;
            
            var tween = go.transform.DOLocalMove(position, duration).SetEase(ease);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }
        
        public async UniTask MoveObject(GameObject go, Vector3 position, float duration)
        {
            if (go == null) return;
            
            var tween = go.transform.DOLocalMove(position, duration);
            _activeTweens.Add(tween);
            
            try
            {
                await tween.ToUniTask();
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }

        public void Dispose()
        {
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive())
                    tween.Kill();
            }
            
            foreach (var sequence in _activeSequences)
            {
                if (sequence != null && sequence.IsActive())
                    sequence.Kill();
            }
            
            _activeTweens.Clear();
            _activeSequences.Clear();
            DOTween.Clear();
        }

        /*[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetAllStatics()
        {
            DOTween.Clear();
        }*/
    }
}