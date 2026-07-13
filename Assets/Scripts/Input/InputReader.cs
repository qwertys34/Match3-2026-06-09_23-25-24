using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputReader : IDisposable
    {
        public event Action Click; // Событие для клика (тапа)
        public event Action<bool, Vector2> Press; // Событие для клика (тапа)
        public event Action<Vector2, Vector2> Swipe; // StartPos, EndPos
        
        private Inputs _inputs;
        private Vector2 _startPos;
        private Vector2 _endPos;
        private bool _isPressed = false;
        private const float SWIPE_THRESHOLD = 30f;
        
        public InputReader()
        {
            _inputs = new Inputs();
            
            // Подписываемся на нажатие/отпускание
            _inputs.Player.Click.started += OnPressStarted;
            _inputs.Player.Click.canceled += OnPressCanceled;
            
            // Подписываемся на движение позиции
            _inputs.Player.Select.performed += OnPositionChanged;
        }
        
        private void OnPressStarted(InputAction.CallbackContext context)
        {
            _isPressed = true;
            _startPos = _inputs.Player.Select.ReadValue<Vector2>();
            Debug.Log($"Нажали в: {_startPos}");
            Press?.Invoke(_isPressed, _startPos);
        }
        
        private void OnPressCanceled(InputAction.CallbackContext context)
        {
            _isPressed = false;
            _endPos = _inputs.Player.Select.ReadValue<Vector2>();
            
            float distance = Vector2.Distance(_startPos, _endPos);
            if (distance > SWIPE_THRESHOLD)
            {
                // Это свайп
                Swipe?.Invoke(_startPos, _endPos);
                Debug.Log($"Свайп: {_startPos} -> {_endPos}");
            }
            else
            {
                // Это клик (тап)
                Click?.Invoke();
                Debug.Log("Клик (короткое касание)");
            }
            Press?.Invoke(_isPressed, _startPos);
        }
        
        private void OnPositionChanged(InputAction.CallbackContext context)
        {
            // Если кнопка НЕ зажата — игнорируем движение
            if (!_isPressed) return;
            
            // Обновляем текущую позицию (можно использовать для отрисовки линии свайпа)
            Vector2 currentPos = context.ReadValue<Vector2>();
            //Press?.Invoke(_isPressed, currentPos);
            // Например: DrawSwipeLine(_startPos, currentPos);
        }
        
        // Метод для получения текущей позиции указателя
        public Vector2 GetPosition()
        {
            return _inputs.Player.Select.ReadValue<Vector2>();
        }
        
        // Метод для включения/отключения ввода
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
            //_inputs.Player.Select.performed -= OnPositionChanged;
        }
    }
}