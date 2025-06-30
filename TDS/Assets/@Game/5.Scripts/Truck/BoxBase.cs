// ----- System 
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace Game
{
    public class BoxBase : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 렌더링 그룹")]
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Material _whiteOutMaterial = null;
        
        [Space(1.5f)]
        [Header("2. 공격 효과 옵션")]
        [SerializeField] private float _fadeInDuration = 0.1f;
        [SerializeField] private float _fadeOutDuration = 0.3f;
        
        [Space(1.5f)]
        [Header("3. 고유 능력 옵션")]
        [SerializeField] private float _hpMax = 1000f;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private Material _originalMaterial = null;
        private Material _whiteEffectMaterial = null;
        private Coroutine _whiteEffectCoroutine = null;

        private float _hpCurrent = 0f;

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        private void Awake()
        {
            InitializeComponents();
        }

        private void OnDestroy()
        {
            if (_whiteEffectMaterial != null)
                DestroyImmediate(_whiteEffectMaterial);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                OnHit();
        }
        
        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        private void InitializeComponents()
        {
            if (_spriteRenderer != null)
            {
                _originalMaterial = _spriteRenderer.material;
                _whiteEffectMaterial = new Material(_whiteOutMaterial);
            }
        }
        
        public void OnHit()
        {
            if (_spriteRenderer != null && _whiteEffectMaterial != null)
            {
                if (_whiteEffectCoroutine != null)
                    StopCoroutine(_whiteEffectCoroutine);
                
                _whiteEffectCoroutine = StartCoroutine(Co_WhiteEffect());
            }
        }
        
        // --------------------------------------------------
        // Methods - Coroutines
        // --------------------------------------------------
        private IEnumerator Co_WhiteEffect()
        {
            _spriteRenderer.material = _whiteEffectMaterial;
            
            yield return StartCoroutine(Co_FadeWhiteAmount(0f, 0.65f, _fadeInDuration));
            yield return StartCoroutine(Co_FadeWhiteAmount(0.65f, 0f, _fadeOutDuration, () => 
            {
                _spriteRenderer.material = _originalMaterial;
                _whiteEffectCoroutine = null;
            }));
            
        }
        
        private IEnumerator Co_FadeWhiteAmount(float startValue, float endValue, float duration, Action doneCallBack = null)
        {
            var elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                var progress = elapsedTime / duration;
                var smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
                var currentValue = Mathf.Lerp(startValue, endValue, smoothProgress);
                
                _whiteEffectMaterial.SetFloat("_WhiteAmount", currentValue);
                
                yield return null;
            }
            
            _whiteEffectMaterial.SetFloat("_WhiteAmount", endValue);
            doneCallBack?.Invoke();
        }
        
    }
}