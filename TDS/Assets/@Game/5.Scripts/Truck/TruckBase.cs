// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace Game
{
    public class TruckBase : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 바퀴 그룹")]
        [SerializeField] private SpriteRenderer[] _wheelSpriteRenderers = new SpriteRenderer[2];

        [Space(1.5f)]
        [Header("2. 바퀴 회전 옵션")]
        [SerializeField] private float _wheelRotationSpeed = 180f; // 초당 회전 각도

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        private void Update()
        {
            RotateWheels();
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        private void RotateWheels()
        {
            // 모든 바퀴를 Z축 기준으로 회전
            for (int i = 0; i < _wheelSpriteRenderers.Length; i++)
            {
                if (_wheelSpriteRenderers[i] != null)
                {
                    Transform wheelTransform = _wheelSpriteRenderers[i].transform;
                    Vector3 currentRotation = wheelTransform.eulerAngles;
                    
                    // Z축 회전값을 시간에 따라 증가
                    currentRotation.z += _wheelRotationSpeed * Time.deltaTime;
                    
                    wheelTransform.eulerAngles = currentRotation;
                }
            }
        }
    }
}