using System.Collections;
using UnityEngine;

namespace Game.Utils
{
    public class FXAutoHide : MonoBehaviour
    {
        private WaitForSeconds _timer = new WaitForSeconds(1f);

        private void OnEnable() => StartCoroutine(HideTimer());

        private IEnumerator HideTimer()
        {
            yield return _timer;
            gameObject.SetActive(false);
        }
    }
}