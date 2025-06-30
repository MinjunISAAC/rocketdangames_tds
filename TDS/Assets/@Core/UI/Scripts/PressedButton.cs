// ----- System
using System.Collections;

// ----- Unity
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class PressedButton : ButtonBase
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Space(1.5f)]
        [Header("3. 클릭 이벤트 옵션")]
        [SerializeField] private float clickInterval = 0.5f;
        [SerializeField] private float clickIntervalFast = 0.03f;
        [SerializeField] private int fastPressCnt = 10;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private float _deltaTime = 0f;
        private int _pressedCnt = 0;
        private Coroutine _currentAnimation = null;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public new bool Interactable
        {
            get => _isInteractable;
            set
            {
                if (_isInteractable != value)
                {
                    if (!value)
                        _pressedCnt = 0;
                }
                _isInteractable = value;
            }
        }

        // --------------------------------------------------
        // Methods - Events
        // --------------------------------------------------
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (_isInteractable == false) return;
            if (_isPressed == false)
                _isPressed = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (!_isInteractable)
                return;

            _isPressed = false;

            if (_pressedCnt < 1)
            {
                Press();
            }

            _pressedCnt = 0;
            _deltaTime = 0f;
        }

        private void OnDisable()
        {
            _isPressed = false;
            _pressedCnt = 0;
            _deltaTime = 0f;
            StopCurrentAnimation();
        }

        public void OnDestroy()
        {
            _isPressed = false;
            _pressedCnt = 0;
            _deltaTime = 0;
            StopCurrentAnimation();
        }

        private void Update()
        {
            if (_isPressed)
            {
                if (!_isInteractable)
                {
                    _isPressed = false;
                    return;
                }

                if ((_pressedCnt < fastPressCnt ? clickInterval : clickIntervalFast) < _deltaTime)
                {
                    if (_pressedCnt < fastPressCnt)
                        DoPressClickAnimation(clickInterval * 0.9f / 2);

                    Press();
                    ++_pressedCnt;
                    _deltaTime = 0f;
                }

                _deltaTime += Time.deltaTime;
            }
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        private void DoPressClickAnimation(float duration = 0.1f)
        {
            StopCurrentAnimation();
            _currentAnimation = StartCoroutine(PressAnimation(duration));
        }

        // --------------------------------------------------
        // Methods - Coroutine
        // --------------------------------------------------
        private IEnumerator PressAnimation(float duration)
        {
            yield return StartCoroutine(ScaleAnimation(Vector3.one, duration));
            yield return StartCoroutine(ScaleAnimation(new Vector3(0.9f, 0.9f, 0.9f), duration));
            _currentAnimation = null;
        }

        private IEnumerator ScaleAnimation(Vector3 targetScale, float duration)
        {
            var startScale = transform.localScale;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            transform.localScale = targetScale;
        }

        private void StopCurrentAnimation()
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
                _currentAnimation = null;
                transform.localScale = Vector3.one;
            }
        }
    }
}