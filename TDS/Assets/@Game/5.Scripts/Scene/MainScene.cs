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
    public class MainScene : SceneBase
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        // [TODO] 테스트를 위한 Chapter 데이터 입니다.
        // 추후 데이터 로드 및 Save Data를 통해 각 스테이지를 로드합니다.
        [SerializeField] private SoChapter _chapter = null;
        [SerializeField] private UnitSpawner _unitSpawner = null;

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
                Debug.Log($"[Initializer.Register_LoadActions] Load Action Start - Chapter Load {_loadIndex}");
                // [TODO] 본 구현에서는 챕터 데이터를 로드하지 않습니다.
                // Resource System을 활용한 로드를 권장합니다. (챕터가 많아지면 많아질 수록 해당 기능은 더 중요해집니다.)
                
                if (_chapter != null)
                {
                    _chapter.Init(); // 챕터 초기화
                    Debug.Log($"[MainScene] 챕터 {_chapter.ChapterIndex} 초기화 완료");
                }
                else
                {
                    Debug.LogError("[MainScene] Chapter가 null입니다!");
                }
                
                InitDone();
            });

            _initActionList.Add(() =>
            {
                Debug.Log($"[Initializer.Register_LoadActions] Load Action Start - Unit Spawner Set {_loadIndex}");
                
                if (_chapter != null)
                {
                    var stageList = _chapter.GetStageList();
                    Debug.Log($"[MainScene] 챕터에서 {stageList?.Count ?? 0}개의 스테이지 로드됨");
                    _unitSpawner.SetUnitGroup(stageList, InitDone);
                }
                else
                {
                    Debug.LogError("[MainScene] Chapter가 null입니다!");
                    InitDone();
                }
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
                _initActionList[_loadIndex].Invoke();
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
    }
}