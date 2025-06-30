// ----- C#
using System.Collections;

// ----- Unity
using UnityEngine;

namespace Core
{
    public abstract class SimpleState<EStateType>
    {
        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private System.Action<EStateType, object> _changeStateCallBack = null;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public abstract EStateType State { get; }

        // --------------------------------------------------
        // Functions - Public Using
        // --------------------------------------------------
        public void Init(System.Action<EStateType, object> changeStateCallBack, object initParam = null)
        {
            Release();

            _changeStateCallBack = changeStateCallBack;

            _Init(initParam);
        }

        public void Release()
        {
            _changeStateCallBack = null;

            _Release();
        }

        public void Start(EStateType preStateType, object startParam = null) { _Start(preStateType, startParam); }
        public void Finish(EStateType nextStateType) { _Finish(nextStateType); }
        public void Update() { _Update(); }

        // --------------------------------------------------
        // Functions - Protected Virtual (Required Implementer)
        // --------------------------------------------------
        protected virtual void _Init(object initParam) { }
        protected virtual void _Release() { }
        protected virtual void _Start(EStateType preStateKey, object startParam) { }
        protected virtual void _Finish(EStateType nextStateKey) { }
        protected virtual void _Update() { }

        // --------------------------------------------------
        // Functions - State
        // --------------------------------------------------
        protected void ChangeState(EStateType nextStateType, object param = null)
        {
            _changeStateCallBack?.Invoke(nextStateType, param);
        }
    }
}