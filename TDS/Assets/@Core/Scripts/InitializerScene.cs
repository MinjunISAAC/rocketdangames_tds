// ----- System
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;
using UI;

namespace Game
{
    public class InitializerScene : SceneBase
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private List<Action> _initActionList = new();

        private int _totalLoadCount = 0;
        private int _loadIndex = 0;

        private bool _isInitialized = false;

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        protected override IEnumerator Start()
        {
            yield return base.Start();
            Register_LoadActions(Execute_LoadActions);
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        protected override void OnInit(Action doneCallBack = null)
        {
            _loadIndex = _totalLoadCount;

            base.OnInit(doneCallBack);

            if (_uiScene.TryGetComponent<UI_InitializerScene>(out var uiInitializerScene))
                uiInitializerScene.EndLoading();
        }

        private void Register_LoadActions(Action doneCallBack)
        {
            _initActionList.Add(() =>
            {
                Debug.Log($"[Initializer.Register_LoadActions] Load Action Start - Scene System {_loadIndex}");
                Owner.Scene.Init(InitDone);
            });

            _initActionList.Add(() =>
            {
                Debug.Log($"[Initializer.Register_LoadActions] Load Action Start - UI System {_loadIndex}");
                Owner.UI.Init(InitDone);
            });

            _initActionList.Add(() =>
            {
                Debug.Log($"[Initializer.Register_LoadActions] Load Action Start - Scene UI Set {_loadIndex}");

                Owner.UI.ShowSceneUI<UI_Scene>($"UI_{Owner.Scene.CurrentSceneType}", (uiScene) =>
                {
                    _uiScene = uiScene;
                    _uiScene.transform.SetParent(Owner.UI.Root.transform);
                    
                    if (_uiScene.TryGetComponent<UI_InitializerScene>(out var uiInitializerScene))
                        uiInitializerScene.StartLoading();
                    
                    InitDone();
                });
            });

            _initActionList.Add(() =>
            {
                Debug.Log($"[Initializer.Register_LoadActions] Load Action Start - UI System {_loadIndex}");
                OnInit(InitDone);
            });

            _totalLoadCount = _initActionList.Count;
            doneCallBack?.Invoke();
        }

        private void Execute_LoadActions()
        {
            if (!_isInitialized)
            {
                _initActionList[_loadIndex].Invoke();
                StartCoroutine(Co_Loading());
            }
            else
                OnInit();
        }

        private void InitDone()
        {
            _loadIndex++;
            Debug.Log($"[Initializer.Register_LoadActions] Load Action Clear {_loadIndex}");

            var sliderValue = _loadIndex / (float)_totalLoadCount;
            var progressValue = (int)(sliderValue * 100f);
            if (_loadIndex < _initActionList.Count)
                _initActionList[_loadIndex].Invoke();
            else
                _isInitialized = true;
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        private IEnumerator Co_Loading()
        {
            yield return new WaitUntil(() => _isInitialized);
            OnInit();
        }
    }
}