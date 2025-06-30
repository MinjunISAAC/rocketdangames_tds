// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace Game
{
    public class MapBase : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 바닥 그룹")]
        [SerializeField] private SpriteRenderer[] _floorSpriteRenderers = new SpriteRenderer[3];

        [Space(1.5f)]
        [Header("2. 이동 옵션")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _resetPositionX = -20f; // 이 X 위치를 지나면 오른쪽으로 재배치

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        private void Start()
        {
            SetupFloorPositions();
        }

        private void Update()
        {
            MoveFloors();
            CheckAndRepositionFloors();
        }
        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        private void SetupFloorPositions()
        {
            var currentPosition = _floorSpriteRenderers[0].transform.position;
            
            for (int i = 1; i < _floorSpriteRenderers.Length && i < 3; i++)
            {
                var prevRenderer = _floorSpriteRenderers[i - 1];
                var prevWidth = prevRenderer.bounds.size.x;
                
                currentPosition.x += prevWidth;
                _floorSpriteRenderers[i].transform.position = currentPosition;
            }
        }

        private void MoveFloors()
        {
            // 모든 바닥을 왼쪽으로 이동
            for (int i = 0; i < _floorSpriteRenderers.Length; i++)
            {
                if (_floorSpriteRenderers[i] != null)
                {
                    Vector3 position = _floorSpriteRenderers[i].transform.position;
                    position.x -= _moveSpeed * Time.deltaTime;
                    _floorSpriteRenderers[i].transform.position = position;
                }
            }
        }

        private void CheckAndRepositionFloors()
        {
            // 가장 왼쪽으로 나간 바닥을 찾아서 오른쪽 끝으로 재배치
            for (int i = 0; i < _floorSpriteRenderers.Length; i++)
            {
                if (_floorSpriteRenderers[i] != null)
                {
                    // 바닥이 재배치 기준점을 넘어갔는지 확인 (바닥의 오른쪽 끝 기준)
                    float rightEdge = _floorSpriteRenderers[i].transform.position.x + (_floorSpriteRenderers[i].bounds.size.x * 0.5f);
                    
                    if (rightEdge < _resetPositionX)
                    {
                        // 가장 오른쪽에 있는 바닥을 찾기
                        SpriteRenderer rightmostFloor = FindRightmostFloor();
                        
                        if (rightmostFloor != null)
                        {
                            // 가장 오른쪽 바닥의 오른쪽 끝에 딱 붙여서 재배치
                            Vector3 newPosition = rightmostFloor.transform.position;
                            newPosition.x += rightmostFloor.bounds.size.x;
                            _floorSpriteRenderers[i].transform.position = newPosition;
                        }
                    }
                }
            }
        }

        private SpriteRenderer FindRightmostFloor()
        {
            SpriteRenderer rightmost = null;
            float maxX = float.MinValue;

            for (int i = 0; i < _floorSpriteRenderers.Length; i++)
            {
                if (_floorSpriteRenderers[i] != null)
                {
                    float currentX = _floorSpriteRenderers[i].transform.position.x;
                    if (currentX > maxX)
                    {
                        maxX = currentX;
                        rightmost = _floorSpriteRenderers[i];
                    }
                }
            }

            return rightmost;
        }
    }
}