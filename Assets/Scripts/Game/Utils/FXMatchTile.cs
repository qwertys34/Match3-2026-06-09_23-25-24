using System.Collections;
using Animations;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Utils
{
    public class FXMatchTile : MonoBehaviour
    {
        private WaitForSeconds _timer = new WaitForSeconds(1f);
        public TMP_Text amountText;
        private IAnimation _animation;

        private void Awake()
        {
            _animation = new AnimationManager();
            amountText.gameObject.SetActive(false);
        }

        private async void OnEnable()
        {
            StartCoroutine(HideTimer());
            amountText.gameObject.SetActive(true);
            _ = _animation.DoPunchAnimate(gameObject, new Vector3(1.1f, 1.2f, 1), 0.3f);
            await _animation.MoveObject(gameObject, transform.position + Vector3.up*0.3f, 0.5f, Ease.OutBack);
            amountText.gameObject.SetActive(false);
        }

        private IEnumerator HideTimer()
        {
            yield return _timer;
            gameObject.SetActive(false);
        }
    }
}