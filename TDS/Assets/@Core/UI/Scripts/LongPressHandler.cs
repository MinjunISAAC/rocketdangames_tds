// ----- C#
using System;
using System.Collections;

// ----- Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI 
{
    public class LongPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 롱프레스 시간 설정")]
        [SerializeField] private float _longPressTime = 2.0f;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Private
        private Button _button = null;
        private float _pressTime = 0f;
        private bool _isLongPressed = false;
        private bool _isPressed = false;
        private bool _isDragging = false;

        // ----- Public
        public Action OnLongPressAction = null;
        public event Action OnLongPressEvent = null;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public bool IsLongPressed => _isLongPressed;

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        public void Awake() { _button = GetComponent<Button>(); }
        
        public void Update() 
        { 
            if (_isPressed && !_isDragging) 
            {
                _pressTime += Time.deltaTime;
                if (_pressTime >= _longPressTime && !_isLongPressed) 
                {
                    OnLongPress();

                    _isPressed = false;
                    _isLongPressed = true;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData) 
        {
            if (!_isPressed)
                StartPress();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this == null || !this.gameObject.activeInHierarchy)
                return;

            StartCoroutine(Co_StartPointerUp());
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        private void StartPress() 
        {
            _isPressed = true;
            _pressTime = 0f;
            _isLongPressed = false;
        }

        private void EndPress() 
        {
            _isPressed = false;
            _pressTime = 0f;
        }

        private void OnLongPress() 
        {
            if (_button != null)
                _button.interactable = false;

            OnLongPressAction?.Invoke();
            OnLongPressEvent?.Invoke();
        }

        // --------------------------------------------------
        // Methods - Coroutine
        // --------------------------------------------------
        private IEnumerator Co_StartPointerUp() 
        {
            EndPress();

            yield return null;

            if (_button != null)
                _button.interactable = true;
        }
    }
}