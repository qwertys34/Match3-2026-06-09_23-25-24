using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Tiles;
using UnityEngine;

namespace Animations
{
    public interface IAnimation
    {
        UniTask Reveal(GameObject target, float delay);
        
        UniTask HideTile(GameObject target);
        
        UniTask DoPunchAnimate(GameObject target, Vector3 scale, float duretion);
        
        void MoveUI(RectTransform target, Vector3 position, float duration, Ease ease);

        UniTask AnimateTile(Tile tile, float value);
        
        void MoveTile(Tile tile, Vector3 position, Ease ease);
        
        UniTask ShakeAnimate(Transform target, float duration, Ease ease);
        
        UniTask MoveObject(GameObject go, Vector3 position, float duration, Ease ease);
        
    }
}