// ----- C#
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI 
{
    public class ButtonBase : UI_Base, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 버튼 활성화 여부")]
        [SerializeField] internal bool _isInteractable = true;
        [SerializeField] internal float _dragThreshold = 10f;

        [Space(1.5f)] [Header("2. 버튼 사운드 및 애니메이션 활성화 여부")]
        [SerializeField] internal bool _isSoundEnabled = true;
        [SerializeField] internal bool _isAnimationEnabled = true;

        [Space(1.5f)] [Header("3. 롱프레스 핸들러")]
        [SerializeField] internal LongPressHandler _longPressHandler = null;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Const
        private const float POINTER_INTERECTION_DURATION = 0.1f;
        private const float POINTER_DOWN_SCALE = 0.9f;
        
        // ----- Private
        private List<ScrollRect> _scrollRects = new();
        private bool _isDragging = false;
        private bool _isOutPosition = false;
        private Vector3 _originScale = Vector3.zero;
        private Coroutine _co_animation = null;

        // ----- Internal
        internal Button.ButtonClickedEvent _onClick = new();
        internal bool _isPressed = false;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public Button.ButtonClickedEvent OnClick 
        {
            get => _onClick;
            set => _onClick = value;
        }

        public bool Interactable 
        {
            get => _isInteractable;
            set => _isInteractable = value;
        }

        // --------------------------------------------------
        // Functions - Event
        // --------------------------------------------------
        public void Awake() 
        { 
            _scrollRects = GetComponentsInParent<ScrollRect>().ToList();
            _originScale = transform.localScale;
        }

        public virtual void OnPointerDown(PointerEventData eventData) 
        {
            _isOutPosition = false;

            if (!_isInteractable)
                return;

            if (_isAnimationEnabled && _co_animation == null)
                _co_animation = StartCoroutine(Co_PointerDown());
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {

        }

        public void OnBeginDrag(PointerEventData eventData) 
        {
        
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }

        public void OnEndDrag(PointerEventData eventData) 
        {
        
        }


        // --------------------------------------------------
        // Functions - Normal
        // --------------------------------------------------
        protected void Press() 
        {
            if (_isOutPosition)
                return;

            if (_scrollRects.Count > 0 && _isDragging)
                return;

            if (_isSoundEnabled)
                PlaySound();

            _onClick?.Invoke();
        }

        protected void PlaySound() 
        {
            
        }

        // --------------------------------------------------
        // Functions - Coroutine
        // --------------------------------------------------
        private IEnumerator Co_PointerDown(Action doneCallBack = null)
        {
            var sec = 0f;
            while(sec < POINTER_INTERECTION_DURATION) 
            {
                sec += Time.deltaTime;
                transform.localScale = Vector3.Lerp(_originScale, _originScale * POINTER_DOWN_SCALE, sec / POINTER_INTERECTION_DURATION);
                yield return null;
            }
            transform.localScale = _originScale * POINTER_DOWN_SCALE;
            doneCallBack?.Invoke();
            _co_animation = null;
        }

        private IEnumerator Co_PointerUp(Action doneCallBack = null)
        {
            var sec = 0f;
            while (sec < POINTER_INTERECTION_DURATION)
            {
                sec += Time.deltaTime;
                transform.localScale = Vector3.Lerp(_originScale * POINTER_DOWN_SCALE, _originScale, sec / POINTER_INTERECTION_DURATION);
                yield return null;
            }
            transform.localScale = _originScale;
            doneCallBack?.Invoke();
            _co_animation = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Press();
        }
    }

    // --------------------------------------------------
    // Editor Code
    // --------------------------------------------------
#if UNITY_EDITOR
    [CustomEditor(typeof(ButtonBase))]
    public class ButtonBaseEditor : UIBaseEditor
    {}
#endif
}