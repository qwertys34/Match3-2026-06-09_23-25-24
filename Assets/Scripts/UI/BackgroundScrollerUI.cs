using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RawImage))]
    public class BackgroundScrollerUI : MonoBehaviour
    {
        private RawImage rawImage;
        private float scrollSpeed = 0.01f;
        private const float DirectionX = -1f;
        private UniTask backgroundTask;
        private void Awake()
        {
            rawImage = GetComponent<RawImage>();
        }

        private async void Start()
        {
            try
            {
                await StartScrollingAsync(destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Нормальное поведение при уничтожении объекта
                Debug.Log("Scrolling cancelled");
            }
            catch (Exception e)
            {
                Debug.Log("ОШИБКА!" + e);
            }
        }

        private async UniTask StartScrollingAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                rawImage.uvRect = new Rect(
                rawImage.uvRect.x + DirectionX * scrollSpeed * Time.deltaTime,
                rawImage.uvRect.y,
                rawImage.uvRect.width,
                rawImage.uvRect.height 
                );
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}