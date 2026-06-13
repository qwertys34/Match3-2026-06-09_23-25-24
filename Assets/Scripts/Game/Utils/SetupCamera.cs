using UnityEngine;

namespace Game.Utils
{
    public class SetupCamera
    {
        private bool _isVertical;

        public void SetCamera(int width, int height,  bool isVertical)
        {
            var xPos = (width / 2f) - 0.5f;
            var yPos = (height / 2f);
            Camera.main.transform.position = new Vector3(xPos, yPos, -11f);
            _isVertical =  isVertical;
            Camera.main.orthographicSize = GetOrthoSize(width, height);
        }

        private float GetOrthoSize(int width, int height)
        {
            return _isVertical ? (width + 1f) * Screen.height / Screen.width * 0.5f 
                : (height + 1f) * Screen.height / Screen.width;
        }
    }
}