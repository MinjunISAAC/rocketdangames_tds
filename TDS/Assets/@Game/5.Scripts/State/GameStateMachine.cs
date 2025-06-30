// ----- C#
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;

namespace Game
{
    public class GameStateMachine : SimpleStateMachine<EStateType>
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
                    _instance._InitSingleton();
                }

                return _instance;
            }
        }

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public override EStateType CurrentState
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
        private void _InitSingleton()
        {
            OnInit
            (
                new Dictionary<EStateType, SimpleState<EStateType>>()
                {
                    { EStateType.Ready, new State_Ready() },
                    { EStateType.Play, new State_Play() },
                    { EStateType.Success, new State_Success() },
                    { EStateType.Fail, new State_Fail() },
                    { EStateType.Pause, new State_Pause() },
                },
                null
            );

            ChangeState(EStateType.Ready);
        }
    }
}