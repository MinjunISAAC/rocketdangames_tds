// ----- System
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace Core
{
    public class SimpleStateMachine<TKey>
    {
        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        protected Dictionary<TKey, SimpleState<TKey>> _stateSet = null;
        protected SimpleState<TKey> _currentState = null;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public virtual TKey CurrentState
        {
            get
            {
                return _currentState.State;
            }
        }

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        // ----- Public
        public virtual void OnInit(Dictionary<TKey, SimpleState<TKey>> stateSet, object param)
        {
            OnRelease();

            if (stateSet == null)
            {
                Debug.LogError("[SimpleStateMachine.OnInit] 초기화 할 State Set이 Null 상태입니다.");
                return;
            }

            _stateSet = stateSet;

            foreach (var targetState in _stateSet)
            {
                var state = targetState.Value;

                if (state == null)
                    continue;

                state.Init(ChangeState, param);
            }
        }

        public virtual void OnUpdate()
        {
            _currentState?.Update();
        }

        public virtual void OnRelease()
        {
            _currentState = null;

            if (_stateSet != null)
            {
                foreach (var statePair in _stateSet)
                {
                    var state = statePair.Value;
                    if (state == null)
                        continue;

                    state.Release();
                }

                _stateSet.Clear();
            }
        }

        public virtual void ChangeState(TKey targetStateType, object startParam = null)
        {
            if (null == _stateSet)
            {
                Debug.LogError("[SimpleStateMachine.ChangeState] State Set이 존재하지 않습니다.");
                return;
            }

            if (!_stateSet.TryGetValue(targetStateType, out var state))
            {
                Debug.LogError($"[SimpleStateMachine.ChangeState] state Set에 {nameof(targetStateType)}가 존재하지 않습니다.");
                return;
            }

            if (null == state)
            {
                Debug.LogError($"[SimpleStateMachine.ChangeState] Target State인 SimpleState[{nameof(targetStateType)}]가 존재하지 않습니다.");
                return;
            }

            TKey prevState = _currentState != null ? _currentState.State : default(TKey);
            
            if (_currentState != null)
            {
                _currentState.Finish(targetStateType);
            }

            _currentState = state;
            _currentState.Start(prevState, startParam);
        }
    }
}