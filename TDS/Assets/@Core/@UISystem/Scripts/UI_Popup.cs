// ----- System
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace UI
{
    public class UI_Popup : UI_Base
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("0. 옵션")]
        [SerializeField] protected RectTransform _rectPanel = null;
        [SerializeField] protected bool _isCloseBackground = true;
        [SerializeField] protected bool _isPlaySound = true;
        [SerializeField] protected bool _isPlayAnimation = true;
        [SerializeField] protected bool _isFullScreen = false;

        // --------------------------------------------------
        // Constants
        // --------------------------------------------------
        private static readonly Vector3 SHOW_MAX_SCALE = new Vector3(1.15f, 1.15f, 1.15f);
        private static readonly Vector3 CLOSE_MAX_SCALE = new Vector3(1.075f, 1.075f, 1.075f);
        private static readonly float SHOW_DURATION = 0.2f;
        private static readonly float TEMP_DURATION = 0.05f;
        private static readonly float CLOSE_DURATION = 0.125f;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private Coroutine _coAnimation = null;


        public event Action OnBeforeCloseAction = null;
        public event Action OnAfterCloseAction = null;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public bool IsPlaySound { get { return _isPlaySound; } }
        public bool IsPlayAnimation { get { return _isPlayAnimation; } }
        public bool IsFullScreen { get { return _isFullScreen; } }

        // --------------------------------------------------
        // Methods - Events
        // --------------------------------------------------
        private void Awake()
        {
            OnAwake();
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public virtual void OnAwake()
        {
            //Systems.UI.SetCanvas(gameObject, true);

            PlayAnimation(true);
            PlaySound();
        }

        public virtual void ClosePopupUI()
        {
            OnBeforeCloseAction?.Invoke();

            if (_isPlayAnimation)
                PlayAnimation(false);
            else
            {
                OnAfterCloseAction?.Invoke();
                Destroy(this.gameObject);
            }
        }

        public virtual void PlaySound()
        {
            // if (_isPlaySound)
            //     Managers.Sound.Play(Define.SoundSourceName.PopupOpen);
        }

        public virtual void PlayAnimation(bool isShow)
        {
            if (_isPlayAnimation)
            {
                if (_coAnimation != null)
                    return;

                if (isShow)
                    _coAnimation = StartCoroutine(Co_ShowAnimation(_rectPanel));
                else
                    _coAnimation = StartCoroutine(Co_CloseAnimation(_rectPanel));
            }
        }

        private IEnumerator Co_ShowAnimation(Transform target, Action doneCallBack = null)
        {
            var elapsedTime = 0f;
            var showDuration = SHOW_DURATION;
            var startScale = Vector3.zero;

            while (elapsedTime < showDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / showDuration;
                target.localScale = Vector3.Lerp(startScale, SHOW_MAX_SCALE, progress);
                yield return null;
            }

            var reduceDuration = TEMP_DURATION;
            var finalScale = Vector3.one;

            elapsedTime = 0f;
            startScale = SHOW_MAX_SCALE;

            while (elapsedTime < reduceDuration)
            {
                elapsedTime += Time.deltaTime;
                var progress = elapsedTime / reduceDuration;
                target.localScale = Vector3.Lerp(startScale, finalScale, progress);
                yield return null;
            }

            target.localScale = finalScale;
            doneCallBack?.Invoke();
            _coAnimation = null;
        }

        private IEnumerator Co_CloseAnimation(Transform target, Action doneCallBack = null)
        {
            var closedTime = 0f;
            var closedDuration = TEMP_DURATION;
            var startScale = Vector3.one;

            while (closedTime < closedDuration)
            {
                closedTime += Time.deltaTime;
                var progress = closedTime / closedDuration;
                target.localScale = Vector3.Lerp(startScale, CLOSE_MAX_SCALE, progress);
                yield return null;
            }

            var closeDuration = CLOSE_DURATION;
            var endScale = Vector3.zero;

            closedTime = 0f;
            startScale = CLOSE_MAX_SCALE;

            while (closedTime < closeDuration)
            {
                closedTime += Time.deltaTime;
                var progress = closedTime / closeDuration;
                target.localScale = Vector3.Lerp(startScale, endScale, progress);
                yield return null;
            }

            OnAfterCloseAction?.Invoke();
            doneCallBack?.Invoke();
            _coAnimation = null;
            Destroy(this.gameObject);
        }
    }
}