// ----- C#
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;

namespace Game
{
    public class GameStateMachine : SimpleStateMachine<Define.EStateType>
    {
        // --------------------------------------------------
        // Singleton
        // --------------------------------------------------
        // ----- Constructor
        private GameStateMachine() { }

        // ----- Static Variables
        private static GameStateMachine _instance = null;

        // ----- Property
        public static GameStateMachine Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new GameStateMachine();
                    _instance.InitSingleton();
                }

                return _instance;
            }
        }

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public override Define.EStateType CurrentState
        {
            get
            {
                return _currentState.State;
            }
        }

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        private class CoroutineExecoutor : MonoBehaviour { }
        private void InitSingleton()
        {
            OnInit
            (
                new Dictionary<Define.EStateType, SimpleState<Define.EStateType>>()
                {
                    { Define.EStateType.Ready, new State_Ready() },
                    { Define.EStateType.Play, new State_Play() },
                    { Define.EStateType.Success, new State_Success() },
                    { Define.EStateType.Fail, new State_Fail() },
                    { Define.EStateType.Pause, new State_Pause() },
                },
                null
            );

            ChangeState(Define.EStateType.Ready);
        }
    }
}