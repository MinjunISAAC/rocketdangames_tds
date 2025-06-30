// ----- System
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_Scene : UI_Base
    {
        // --------------------------------------------------
        // Methods - Events
        // --------------------------------------------------
        protected virtual void Awake()
        {
            Owner.UI.SetCanvas(gameObject, false);
        }
    }
}