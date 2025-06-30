// ----- System
using System;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEngine.UI;

// ----- User Defined
using Core;

namespace UI
{
    public class UISystem
    {
        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private int _canvasOrder = 10;
        private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
        private UI_Scene _currentSceneUI = null;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public int PopupCount
        {
            get
            {
                return _popupStack.Count;
            }
        }

        public int CanvasOrder
        {
            get
            {
                return _canvasOrder;
            }
        }

        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find(Define.UI_ROOT);

                if (root == null)
                    root = new GameObject { name = Define.UI_ROOT };

                return root;
            }
        }

        public GameObject DondestroyRoot
        {
            get
            {
                GameObject root = GameObject.Find(Define.UI_GLOBAL_ROOT);

                if (root == null)
                    root = new GameObject { name = Define.UI_GLOBAL_ROOT };

                GameObject.DontDestroyOnLoad(root);
                return root;
            }
        }

        // --------------------------------------------------
        // Methods - Manage Init
        // --------------------------------------------------
        public void Init(Action doneCallBack = null)
        {
            var root = GameObject.Find(Define.UI_ROOT);
            if (root == null)
                root = new GameObject { name = Define.UI_ROOT };

            var globalRoot = GameObject.Find(Define.UI_GLOBAL_ROOT);
            if (globalRoot == null)
            {
                globalRoot = new GameObject { name = Define.UI_GLOBAL_ROOT };
                GameObject.DontDestroyOnLoad(globalRoot);
            }

            doneCallBack?.Invoke();
        }

        public void Clear()
        {
            _popupStack.Clear();
        }

        // --------------------------------------------------
        // Methods - UI Init
        // --------------------------------------------------
        public void SetCanvas(GameObject go, bool popup = true)
        {
            var canvas = go.GetComponent<Canvas>();
            var canvasScaler = go.GetComponent<CanvasScaler>();
            var graphicRaycaster = go.GetComponent<GraphicRaycaster>();

            if (canvas == null)
                canvas = go.AddComponent<Canvas>();

            if (canvasScaler == null)
                canvasScaler = go.AddComponent<CanvasScaler>();

            if (graphicRaycaster == null)
                graphicRaycaster = go.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            if (popup)
            {
                var uiPopup = go.GetComponent<UI_Popup>();

                if (uiPopup == null)
                    uiPopup = go.AddComponent<UI_Popup>();

                _canvasOrder += 1;
                canvas.sortingOrder = _canvasOrder;
            }
            else
                canvas.sortingOrder = 1;

            var setWidth = 1080;
            var setHeight = 1920;
            var deviceWidth = Screen.width;
            var deviceHeight = Screen.height;

            if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight)
                canvasScaler.matchWidthOrHeight = 1;
            else
                canvasScaler.matchWidthOrHeight = 0;
        }

        #region [Popup Group]
        public void ShowPopupUI<T>(string name = null, Action<T> callback = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            Systems.Resource.LoadAsync<GameObject>($"Prefabs/UI/Popups/UI_{name}.prefab", Define.ELoadType.Global, (obj) =>
            {
                GameObject go = Systems.Resource.Instantiate($"Prefabs/UI/Popups/UI_{name}.prefab");

                if (go != null)
                {
                    if (!go.TryGetComponent<T>(out T popup))
                        popup = go.AddComponent<T>();
                    else
                    {
                        _popupStack.Push(popup);
                        go.transform.SetParent(Root.transform);
                    }

                    callback?.Invoke(popup);
                }
            });
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0)
                return;

            var popup = _popupStack.Pop();

            _canvasOrder -= 1;

            popup.ClosePopupUI();
        }

        public void CloseAllPopupUI()
        {
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }
        #endregion

        #region [Scene Group]
        public void ShowSceneUI<T>(string name = null, Action<T> callback = null) where T : UI_Scene
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            Systems.Resource.LoadAsync<GameObject>($"Prefabs/UI/Scenes/{name}.prefab", Define.ELoadType.Global, (obj) =>
            {
                GameObject go = Systems.Resource.Instantiate($"Prefabs/UI/Scenes/{name}.prefab");

                if (go != null)
                {
                    if (!go.TryGetComponent<T>(out T scene))
                    {
                        _currentSceneUI = go.AddComponent<T>();
                        callback?.Invoke(scene);
                    }
                    else
                    {
                        _currentSceneUI = scene;
                        callback?.Invoke(scene);
                    }
                }
            });
        }

        public void GetSceneUI<T>(Action<T> callback) where T : UI_Scene
        {
            if (_currentSceneUI == null)
                return;

            if (_currentSceneUI is T sceneUI)
                callback?.Invoke(sceneUI);
        }

        #endregion
    }
}