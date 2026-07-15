using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputReader : IDisposable
    {
        public event Action Click; 
        public event Action<Vector2, Vector2> Swipe; 
        
        private Inputs _inputs;
        private Vector2 _startPos;
        private Vector2 _endPos;
        private const float SWIPE_THRESHOLD = 30f;
        
        public InputReader()
        {
            _inputs = new Inputs();
            
            _inputs.Player.Click.started += OnPressStarted;
            _inputs.Player.Click.canceled += OnPressCanceled;
        }
        
        private void OnPressStarted(InputAction.CallbackContext context)
        {
            _startPos = _inputs.Player.Select.ReadValue<Vector2>();
        }
        
        private void OnPressCanceled(InputAction.CallbackContext context)
        {
            _endPos = _inputs.Player.Select.ReadValue<Vector2>();
            
            float distance = Vector2.Distance(_startPos, _endPos);
            if (distance > SWIPE_THRESHOLD)
            {
                Swipe?.Invoke(_startPos, _endPos);
                Debug.Log($"Свайп: {_startPos} -> {_endPos}");
            }
            else
            {
                Click?.Invoke();
                Debug.Log("Клик (короткое касание)");
            }
        }
        
        public Vector2 GetPosition()
        {
            return _inputs.Player.Select.ReadValue<Vector2>();
        }
        
        public void EnableInput(bool value)
        {
            if (value)
                _inputs.Player.Enable();
            else
                _inputs.Player.Disable();
        }
        
        public void Dispose()
        {
            _inputs.Player.Click.started -= OnPressStarted;
            _inputs.Player.Click.canceled -= OnPressCanceled;
        }
    }
}