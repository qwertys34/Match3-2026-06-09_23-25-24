using Game.Utils;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Grid = Game.GridSystem.Grid;

namespace Game.UI
{
    public class SetupGameUI : MonoBehaviour
    {
        [SerializeField] private RectTransform gameProgressPanel;
        [SerializeField] private Vector3 verticalPos;
        [SerializeField] private Vector3 horizontalPos;
        [SerializeField] private Sprite horizontalBackground;
        [SerializeField] private Sprite verticalBackground;
        [SerializeField] private RawImage currentBackground;
        private Grid _grid;
        private SetupCamera _setupCamera;
        private DeviceOrientation currentOrientation;
        private DeviceOrientation newOrientation;
    
        [Inject] private void Conctruct(Grid grid, SetupCamera setupCamera)
        {
            _grid = grid;
            _setupCamera = setupCamera;
        }

        private void Start()
        {
            Debug.Log(_setupCamera.IsVertical);
            bool isVertical = false;
            currentOrientation = GetCurrentOrientation(ref isVertical);
            currentBackground.texture = isVertical ? verticalBackground.texture :
                horizontalBackground.texture;    
        }
    
        DeviceOrientation GetCurrentOrientation(ref bool isVertical)
        {
            DeviceOrientation orientation = UnityEngine.Input.deviceOrientation;
        
            if (orientation == DeviceOrientation.Unknown)
            {
                if (Screen.width > Screen.height)
                {
                    isVertical = false;
                    return DeviceOrientation.LandscapeLeft;
                }
                else
                {
                    isVertical = true;
                    return DeviceOrientation.Portrait;
                }
            }

            isVertical = orientation == DeviceOrientation.Portrait;
            return orientation;
        }
    
        DeviceOrientation GetCurrentOrientation()
        {
            DeviceOrientation orientation = UnityEngine.Input.deviceOrientation;
        
            if (orientation == DeviceOrientation.Unknown)
            {
                if (Screen.width > Screen.height)
                {
                    return DeviceOrientation.LandscapeLeft;
                }
                else
                {
                    return DeviceOrientation.Portrait;
                }
            }

            return orientation;
        }

        /*private void SetGameProgressPanel(bool isVertical)
        {
            if (isVertical)
            {
                gameProgressPanel.localPosition = verticalPos;
                currentBackground.texture = verticalBackground.texture;
                //gameProgressPanel.localRotation = 
            }
            else
            {
                gameProgressPanel.localPosition = horizontalPos;
                currentBackground.texture = horizontalBackground.texture;
            }
        }*/

        private void Update()
        {
            newOrientation = GetCurrentOrientation();
            if (currentOrientation != newOrientation)
            {
                //SetGameProgressPanel(!_setupCamera.IsVertical);
                _setupCamera.SetCamera(_grid.Width, _grid.Height, !_setupCamera.IsVertical);
                currentOrientation = GetCurrentOrientation();
                currentBackground.texture = _setupCamera.IsVertical ? verticalBackground.texture :
                    horizontalBackground.texture;    
            }
        }
    }
}
