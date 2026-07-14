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
        
        void DoPunchAnimate(GameObject target, Vector3 scale, float duretion);
        
        void MoveUI(RectTransform target, Vector3 position, float duration, Ease ease);

        void AnimateTile(Tile tile, float value);
        
        void MoveTile(Tile tile, Vector3 position, Ease ease);
        
        void MoveObject(GameObject go, Vector3 position, float duration, Ease ease);
        
    }
}