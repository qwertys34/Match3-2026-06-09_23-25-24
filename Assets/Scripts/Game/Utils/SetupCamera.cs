using UnityEngine;

namespace Game.Utils
{
    public class SetupCamera
    {
        public bool IsVertical { get; private set; }

        public void SetCamera(int width, int height,  bool isVertical)
        {
            var xPos = (width / 2f) - 0.5f;
            var yPos = (height / 2f) + 0.2f;
            if (Camera.main == null) return;
            Camera.main.transform.position = new Vector3(xPos, yPos, -11f);
            this.IsVertical = isVertical;
            Camera.main.orthographicSize = GetOrthoSize(width, height);
        }

        private float GetOrthoSize(int width, int height)
        {
            return IsVertical ? (width + 1f) * Screen.height / Screen.width * 0.5f 
                : (height + 1f) * Screen.height / (Screen.width / 1.2f);
        }
    }
}