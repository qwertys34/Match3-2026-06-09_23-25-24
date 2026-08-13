using System.Collections;
using UnityEngine;

namespace UI
{
    public class SetupUI : MonoBehaviour
    {
        [SerializeField] private RectTransform[] buttons;
        private DeviceOrientation currentOrientation;
        
        void Start()
        {
            StartCoroutine(InitializeOrientation());
        }

        IEnumerator InitializeOrientation()
        {
            yield return null;
            
            yield return new WaitForSeconds(1.5f);
            
            currentOrientation = GetCurrentOrientation();
            UpdateUIForOrientation(currentOrientation);
            
            yield return new WaitForSeconds(0.5f);
            UpdateUIForOrientation(GetCurrentOrientation());
        }

        void Update()
        {
            DeviceOrientation newOrientation = GetCurrentOrientation();
            if (newOrientation != currentOrientation)
            {
                currentOrientation = newOrientation;
                UpdateUIForOrientation(currentOrientation);
            }
        }

        DeviceOrientation GetCurrentOrientation()
        {
            DeviceOrientation orientation = UnityEngine.Input.deviceOrientation;
            
            if (orientation == DeviceOrientation.Unknown)
            {
                if (Screen.width > Screen.height)
                    return DeviceOrientation.LandscapeLeft;
                else
                    return DeviceOrientation.Portrait;
            }
            
            return orientation;
        }
    
        void UpdateUIForOrientation(DeviceOrientation orientation)
        {
            if (orientation == DeviceOrientation.Unknown)
            {
                orientation = DeviceOrientation.Portrait;
            }
            
            switch (orientation)
            {
                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].localScale = new Vector3(0.86f, 0.86f, 0.86f);
                        var locPos = buttons[i].localPosition;
                        locPos.y = -555f;
                        if (i == 0) locPos.x = -363f;
                        else if (i == 1) locPos.x = -175f;
                        else if (i == 3) locPos.x = 175f;
                        else if (i == 4) locPos.x = 363f;
                        buttons[i].localPosition = locPos;
                    }
                    break;
                
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].localScale = new Vector3(1, 1, 1);
                        var locPos = buttons[i].localPosition;
                        locPos.y = -300f;
                        if (i == 0) locPos.x = -400f;
                        else if (i == 1) locPos.x = -195f;
                        else if (i == 3) locPos.x = 195f;
                        else if (i == 4) locPos.x = 400f;
                        buttons[i].localPosition = locPos;
                    }
                    break;
            }
        }
    }
}