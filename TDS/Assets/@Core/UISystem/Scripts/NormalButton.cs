// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class NormalButton : ButtonBase
    {
        // --------------------------------------------------
        // Methods - Events
        // --------------------------------------------------
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (_isInteractable == false) 
                return;
            
            if (_isPressed == false)
                _isPressed = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (!_isInteractable) 
                return;

            if (_isPressed == false) 
                return;
            
            _isPressed = false;
            
            if (_longPressHandler == null || !_longPressHandler.IsLongPressed)
                Press();
        }
    }
}
