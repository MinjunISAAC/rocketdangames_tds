// ----- C#
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;

namespace Game 
{
    public class State_Fail : SimpleState<EStateType>
    {
        // --------------------------------------------------
        // Property
        // --------------------------------------------------
        public override EStateType State => EStateType.Fail;

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        protected override void _Start(EStateType preStateKey, object startParam)
        {
            Debug.Log($"[State_{GameStateMachine.Instance.CurrentState}._Start] {GameStateMachine.Instance.CurrentState} State에 진입하였습니다.");
        }

        protected override void _Update() 
        { 
        
        }

        protected override void _Finish(EStateType nextStateKey)
        {
            Debug.Log($"[State_{GameStateMachine.Instance.CurrentState}._Finish] {GameStateMachine.Instance.CurrentState} State에서 빠져나왔습니다.");
        }
    }
}