// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ----- User Define
using UI;
using Core;

namespace Game
{
    public class UI_InitializerScene : UI_Scene
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 로딩 그룹")]
        [SerializeField] private Slider _loadingSlider = null;
        [SerializeField] private TextMeshProUGUI _TMP_Loading = null;

        [Space(1.5f)]
        [Header("2. 버튼 그룹")]
        [SerializeField] private NormalButton _NBTN_Start = null;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private const string LOADING_TEXT = "Loading...";
        private const string LOADING_VALUE_TEXT = "({0}%/100%)";

        private RectTransform _loadingRect = null;
        private float _maxLoadingWidth = 0f;

        // --------------------------------------------------
        // Methods - Events
        // --------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            if (_loadingSlider.TryGetComponent<RectTransform>(out var rect))
            {
                _loadingRect = rect;
                _maxLoadingWidth = _loadingRect.sizeDelta.x;
            }

            _NBTN_Start.gameObject.SetActive(false);

            SetButtonEvents();
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        #region [Loading]
        public void StartLoading()
        {
            //_IMG_LoadingSlider.gameObject.TrySetActive(true);
            //_TMP_Loading.gameObject.TrySetActive(true);
            _NBTN_Start.gameObject.SetActive(false);
            SetLoading(0);
        }

        public void EndLoading()
        {
            //_IMG_LoadingSlider.gameObject.TrySetActive(false);
            //_TMP_Loading.gameObject.TrySetActive(false);
            _NBTN_Start.gameObject.SetActive(true);
            SetLoading(100);
        }

        public void SetLoading(float value)
        {
            var loadingValue = Mathf.Round((float)value * 1000f) / 10f;

            _loadingRect.sizeDelta = new Vector2(_maxLoadingWidth * value, _loadingRect.sizeDelta.y);
            _TMP_Loading.text = LOADING_TEXT + " " + string.Format(LOADING_VALUE_TEXT, loadingValue);
        }
        #endregion

        #region [Button Events]
        private void SetButtonEvents()
        {
            _NBTN_Start.OnClick.AddListener(OnClickStartButton);
        }

        private void OnClickStartButton()
        {
            Debug.Log($"[UI_InitializerScene.OnClickStartButton] Start Button Clicked");
            Owner.Scene.LoadScene(Define.ESceneType.MainScene);
        }
        #endregion
    }
}