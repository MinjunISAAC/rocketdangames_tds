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
        [Header("1. 배경 그룹")]
        [SerializeField] private SpriteRenderer[] _floorSpriteRenderers = new SpriteRenderer[3];
        [SerializeField] private SpriteRenderer[] _backgroundSpriteRenderers = new SpriteRenderer[3];

        [Space(1.5f)]
        [Header("2. 바닥 이동 옵션")]
        [SerializeField] private float _floorMoveSpeed = 5f;
        [SerializeField] private float _resetPositionX = -20f; // 이 X 위치를 지나면 오른쪽으로 재배치

        [Space(1.5f)]
        [Header("3. 배경 이동 옵션")]
        [SerializeField] private float[] _backgroundMoveSpeeds = new float[3] { 2f, 3f, 4f }; // 각 배경 레이어별 속도

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        private void Start()
        {
            SetupFloorPositions();
            SetupBackgroundPositions();
        }

        private void Update()
        {
            MoveFloors();
            CheckAndRepositionFloors();
            MoveBackgrounds();
            CheckAndRepositionBackgrounds();
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
                    position.x -= _floorMoveSpeed * Time.deltaTime;
                    _floorSpriteRenderers[i].transform.position = position;
                }
            }
        }

        private void SetupBackgroundPositions()
        {
            if (_backgroundSpriteRenderers == null || _backgroundSpriteRenderers.Length == 0)
                return;

            var currentPosition = _backgroundSpriteRenderers[0].transform.position;
            
            for (int i = 1; i < _backgroundSpriteRenderers.Length && i < 3; i++)
            {
                if (_backgroundSpriteRenderers[i] != null && _backgroundSpriteRenderers[i - 1] != null)
                {
                    var prevRenderer = _backgroundSpriteRenderers[i - 1];
                    var prevWidth = prevRenderer.bounds.size.x;
                    
                    currentPosition.x += prevWidth;
                    _backgroundSpriteRenderers[i].transform.position = currentPosition;
                }
            }
        }

        private void MoveBackgrounds()
        {
            // 각 배경을 다른 속도로 왼쪽으로 이동
            for (int i = 0; i < _backgroundSpriteRenderers.Length; i++)
            {
                if (_backgroundSpriteRenderers[i] != null)
                {
                    // 속도 배열의 범위를 벗어나지 않도록 확인
                    float moveSpeed = (i < _backgroundMoveSpeeds.Length) ? _backgroundMoveSpeeds[i] : _backgroundMoveSpeeds[0];
                    
                    Vector3 position = _backgroundSpriteRenderers[i].transform.position;
                    position.x -= moveSpeed * Time.deltaTime;
                    _backgroundSpriteRenderers[i].transform.position = position;
                }
            }
        }

        private void CheckAndRepositionBackgrounds()
        {
            // 가장 왼쪽으로 나간 배경을 찾아서 오른쪽 끝으로 재배치
            for (int i = 0; i < _backgroundSpriteRenderers.Length; i++)
            {
                if (_backgroundSpriteRenderers[i] != null)
                {
                    // 배경이 재배치 기준점을 넘어갔는지 확인 (배경의 오른쪽 끝 기준)
                    float rightEdge = _backgroundSpriteRenderers[i].transform.position.x + (_backgroundSpriteRenderers[i].bounds.size.x * 0.5f);
                    
                    if (rightEdge < _resetPositionX)
                    {
                        // 가장 오른쪽에 있는 배경을 찾기
                        SpriteRenderer rightmostBackground = FindRightmostBackground();
                        
                        if (rightmostBackground != null)
                        {
                            // 가장 오른쪽 배경의 오른쪽 끝에 딱 붙여서 재배치
                            Vector3 newPosition = rightmostBackground.transform.position;
                            newPosition.x += rightmostBackground.bounds.size.x;
                            _backgroundSpriteRenderers[i].transform.position = newPosition;
                        }
                    }
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

        private SpriteRenderer FindRightmostBackground()
        {
            SpriteRenderer rightmost = null;
            float maxX = float.MinValue;

            for (int i = 0; i < _backgroundSpriteRenderers.Length; i++)
            {
                if (_backgroundSpriteRenderers[i] != null)
                {
                    float currentX = _backgroundSpriteRenderers[i].transform.position.x;
                    if (currentX > maxX)
                    {
                        maxX = currentX;
                        rightmost = _backgroundSpriteRenderers[i];
                    }
                }
            }

            return rightmost;
        }
    }
}