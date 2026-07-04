using UnityEngine;

namespace SceneLoading
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;

        public void SetActiveScreen(bool value) => loadingScreen.SetActive(value);
    }
}