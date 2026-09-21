using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Tiles;
using UnityEngine;

namespace Animations
{
    public interface IAnimation
    {
        UniTask Reveal(GameObject target, float delay, float size = 1f);
        UniTask RevealUI(GameObject target, float delay, Vector3 scale);
        
        UniTask HideTile(GameObject target);
        UniTask HideTileUI(GameObject target, float duretion = 0.08f);
        
        UniTask DoPunchAnimate(GameObject target, Vector3 scale, float duretion);
        
        void MoveUI(RectTransform target, Vector3 position, float duration, Ease ease);

        UniTask MoveUIWithCanceletion(RectTransform target, Vector3 position, float duration, Ease ease,
            CancellationTokenSource cts);

        UniTask AnimateTile(Tile tile, float value, float duretion = 0.3f);
        
        void MoveTile(Tile tile, Vector3 position, Ease ease);
        
        UniTask ShakeAnimate(Transform target, float duration, Ease ease);
        UniTask ShakeAnimateUntil(Transform target, Ease ease, CancellationTokenSource cts);
        
        UniTask MoveObject(GameObject go, Vector3 position, float duration, Ease ease);
        UniTask MoveObject(GameObject go, Vector3 position, float duration);

        UniTask AnimateSuperCandyMatch(Transform target, SpriteRenderer sr,
            float value, float duration);
    }
}