using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Input
{
    public class InputReader : IDisposable
    {
        public event Action Click; 
        public event Action<Vector2, Vector2> Swipe;
        public event Action Pause;
        public event Action UIClick;
        
        private Inputs _inputs;
        private Vector2 _startPos;
        private Vector2 _endPos;
        private const float SWIPE_THRESHOLD = 30f;
        
        private Button _pauseButton;
        
        public static bool IsVectical()
        {
            DeviceOrientation orientation = UnityEngine.Input.deviceOrientation;
            
            if (orientation == DeviceOrientation.Unknown)
            {
                if (Screen.width > Screen.height)
                    return false;
                else
                    return true;
            }
            
            return orientation == DeviceOrientation.Portrait;
        }
        
        public InputReader()
        {
            _inputs = new Inputs();

            _inputs.Player.Click.Enable();
                
            _inputs.Player.Click.started += OnPressStarted;
            _inputs.Player.Click.canceled += OnPressCanceled;
            
            _inputs.Player.Pause.Enable();
            _inputs.Player.Pause.performed += OnGamePause;
        }

        private void OnGamePause(InputAction.CallbackContext context)
        {
            Debug.Log("escape pressed");
            Pause?.Invoke();
        }

        private void OnPressStarted(InputAction.CallbackContext context)
        {
            _startPos = _inputs.Player.Select.ReadValue<Vector2>(); 
            UIClick?.Invoke();
        }
        
        private void OnPressCanceled(InputAction.CallbackContext context)
        {
            _endPos = _inputs.Player.Select.ReadValue<Vector2>();
            
            float distance = Vector2.Distance(_startPos, _endPos);
            if (distance > SWIPE_THRESHOLD)
            {
                Swipe?.Invoke(_startPos, _endPos);
            }
            else
            {
                Click?.Invoke();
            }
            
            UIClick?.Invoke();
        }
        
        public Vector2 GetPosition()
        {
            return _inputs.Player.Select.ReadValue<Vector2>();
        }
        
        public void EnablePlayerInput(bool value)
        {
            if (value)
            {
                _inputs.Player.Enable();
            }
            else
            {
                _inputs.Player.Disable();
            }
        }
        
        public void Dispose()
        {
            _inputs.Player.Click.started -= OnPressStarted;
            _inputs.Player.Click.canceled -= OnPressCanceled;
            _inputs.Player.Pause.started -= OnGamePause;
        }
    }
}