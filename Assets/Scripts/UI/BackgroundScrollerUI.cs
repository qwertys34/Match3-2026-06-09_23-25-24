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
        
        private void Awake()
        {
            rawImage = GetComponent<RawImage>();
        }

        private async void Start()
        {
            await StartScrollingAsync().SuppressCancellationThrow();
        }

        private async UniTask StartScrollingAsync()
        {
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                rawImage.uvRect = new Rect(
                rawImage.uvRect.x + DirectionX * scrollSpeed * Time.deltaTime,
                rawImage.uvRect.y,
                rawImage.uvRect.width,
                rawImage.uvRect.height 
                );
                await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
            }
        }
    }
}