// ----- System
using System;
using System.Collections;

// ----- Unity
using UnityEngine;

// ----- User Defined
using UI;

namespace Core
{
    public class SceneBase : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. UI 그룹")]
        [SerializeField] protected UI_Scene _uiScene = null;

        // --------------------------------------------------
        // Methods - Events
        // --------------------------------------------------
        protected virtual void Awake() { }

        protected virtual IEnumerator Start()
        {
            Debug.Log($"[SceneBase.Start] {Owner.Scene.CurrentSceneType} 씬 입장");
            yield return null;
        }

        public void OnDestroy()
        {
            //Systems.Scene.OnInit = null;
        }

        public void OnDisable()
        {
            //Systems.Scene.OnInit = null;
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        protected virtual void OnInit(Action doneCallBack = null)
        {
            Debug.Log($"[SceneBase.OnInit] {Owner.Scene.CurrentSceneType} 씬 입장");
            doneCallBack?.Invoke();
        }

        public virtual void OnInitComplete()
        {
            //Systems.UI.CloseLoadingUI();
        }
    }
}