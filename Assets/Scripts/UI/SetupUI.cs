using UnityEngine;

namespace UI
{
    public class SetupUI : MonoBehaviour
    {
        [SerializeField] private RectTransform[] buttons;
        private DeviceOrientation currentOrientation;

        void Start()
        {
            currentOrientation = UnityEngine.Input.deviceOrientation;
            UpdateUIForOrientation(currentOrientation);
        }
        
        void Update()
        {
            if (UnityEngine.Input.deviceOrientation != currentOrientation)
            {
                currentOrientation = UnityEngine.Input.deviceOrientation;
                UpdateUIForOrientation(currentOrientation);
            }
        }
    
        void UpdateUIForOrientation(DeviceOrientation orientation)
        {
            switch (orientation)
            {
                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].localScale = new Vector3(1, 1, 1);
                        var locPos = buttons[i].localPosition;
                        if (i == 0) locPos.x -= 50;
                        else if (i == 1) locPos.x -= 20;
                        else if (i == 2) continue;
                        else if (i == 3) locPos.x += 20;
                        else if (i == 4) locPos.x += 50;
                        buttons[i].position = locPos;
                    }
                    break;
                
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].localScale = new Vector3(0.86f, 0.86f, 0.86f);
                        var locPos = buttons[i].localPosition;
                        if (i == 0) locPos.x += 50;
                        else if (i == 1) locPos.x += 20;
                        else if (i == 2) continue;
                        else if (i == 3) locPos.x -= 20;
                        else if (i == 4) locPos.x -= 50;
                        buttons[i].position = locPos;
                    }
                    break;
            }
        }
    }
}
