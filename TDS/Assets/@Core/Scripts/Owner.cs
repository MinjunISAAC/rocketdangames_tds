// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Define
using Core;
using UI;

public class Owner : MonoBehaviour
{
    // --------------------------------------------------
    // Core Manage Group
    // --------------------------------------------------

    // --------------------------------------------------
    // Properties
    // --------------------------------------------------
    private static Owner _instance;
    public static bool IsInit = false;
    public static Owner Instance
    {
        get
        {
            if (!IsInit)
            {
                IsInit = true;
                if (_instance == null)
                {
                    GameObject go = GameObject.Find($"@{Define.OWNER}");
                    if (go == null)
                    {
                        go = new GameObject { name = $"@{Define.OWNER}" };
                        go.AddComponent<Owner>();
                    }
                    DontDestroyOnLoad(go);
                    _instance = go.GetComponent<Owner>();
                }
                InitForce();
            }
            return _instance;
        }
    }

    // --------------------------------------------------
    // Methods - Events
    // --------------------------------------------------
    private void Start()
    {

    }

    // --------------------------------------------------
    // Methods - Normal
    // --------------------------------------------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        if (IsInit == false)
        {
            IsInit = true;
            if (Instance == null)
            {
                GameObject go = GameObject.Find($"@{Define.OWNER}");
                if (go == null)
                {
                    go = new GameObject { name = $"@{Define.OWNER}" };
                    go.AddComponent<Owner>();
                }

                DontDestroyOnLoad(go);
                _instance = go.GetComponent<Owner>();
            }

            InitForce();
        }
    }

    private static void InitForce()
    {
        
    }
}