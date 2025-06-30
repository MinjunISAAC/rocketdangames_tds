// ----- System
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEngine.SceneManagement;

// ----- User Defined
using UI;

namespace Core
{
    public class SceneLoadSystem
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 상황 확인")]
        [SerializeField] private Define.ESceneType _currenctSceneType = Define.ESceneType.Unknown;
        [SerializeField] private Define.ESceneType _previousSceneType = Define.ESceneType.Unknown;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private bool _isRunLoading = false;
        private float _time = 0;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public Define.ESceneType CurrentSceneType => _currenctSceneType;
        public Define.ESceneType PreviousSceneType => _previousSceneType;

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public void Init(Action doneCallBack = null)
        {
            var currSceneName = SceneManager.GetActiveScene().name;
            _currenctSceneType = (Define.ESceneType)Enum.Parse(typeof(Define.ESceneType), currSceneName);

            doneCallBack?.Invoke();
        }

        public void LoadScene(Define.ESceneType sceneType, Action callback = null)
        {
            LoadScene(sceneType.ToString(), callback);
        }

        public void LoadScene(string SceneName, Action callback = null)
        {
            if (_isRunLoading)
                return;

            var currSceneName = SceneManager.GetActiveScene().name;
            _previousSceneType = (Define.ESceneType)Enum.Parse(typeof(Define.ESceneType), currSceneName);
            _currenctSceneType = (Define.ESceneType)Enum.Parse(typeof(Define.ESceneType), SceneName);

            Owner.Instance.StartCoroutine(Co_LoadAsyncSceneCoroutine(SceneName, callback));
        }

        // --------------------------------------------------
        // Functions - Coroutine
        // --------------------------------------------------
        private IEnumerator Co_LoadAsyncSceneCoroutine(string SceneName, Action callback)
        {
            _isRunLoading = true;

            yield return new WaitForSeconds(0.1f);

            AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);
            operation.allowSceneActivation = false;

            _time = 0;

            while (_time < 0.09f)
            {
                _time += Time.deltaTime;
                if (_time > 0.09f)
                    operation.allowSceneActivation = true;
                yield return null;
            }

            yield return new WaitUntil(() => operation.isDone);
            if (operation.isDone)
                callback?.Invoke();

            _isRunLoading = false;
        }
    }
}