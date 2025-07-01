// ----- System
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;

namespace Game
{
    public class UnitSpawner : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 레이어 그룹")]
        [SerializeField] private LayerMask[] _unitLayerMasks = new LayerMask[3];
        [SerializeField] private LayerMask[] _groundLayerMasks = new LayerMask[3];

        [Space(1.5f)] [Header("2. 스폰 그룹")]
        [SerializeField] private Transform _spawnPoint = null;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private Dictionary<int, SoStage> _stageDataSet = new();
        private Coroutine _currentSpawnCoroutine = null;

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        public void Update()
        {
            // 테스트용 키 입력
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartStage(0);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                StartStage(1);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                StartStage(2);
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public void SetUnitGroup(List<SoStage> stageDataSet, Action doneCallBack)
        {
            _stageDataSet.Clear(); // 기존 데이터 클리어
            
            foreach (var stageData in stageDataSet)
            {
                if (!_stageDataSet.ContainsKey(stageData.StageIndex))
                {
                    _stageDataSet.Add(stageData.StageIndex, stageData);
                    Debug.Log($"[UnitSpawner] 스테이지 {stageData.StageIndex} 추가됨");
                }
                else
                {
                    Debug.LogWarning($"[UnitSpawner] 스테이지 {stageData.StageIndex}는 이미 존재합니다. 덮어쓰기 됩니다.");
                    _stageDataSet[stageData.StageIndex] = stageData; // 덮어쓰기
                }
            }

            Debug.Log($"[UnitSpawner] 유닛 그룹 설정 완료 {_stageDataSet.Count}");
            doneCallBack?.Invoke();
        }

        public void StartStage(int stageIndex)
        {
            if (_currentSpawnCoroutine != null)
                StopCoroutine(_currentSpawnCoroutine);

            if (_stageDataSet.TryGetValue(stageIndex, out SoStage stageData))
            {
                Debug.Log($"[UnitSpawner] 스테이지 {stageIndex} 시작!");
                _currentSpawnCoroutine = StartCoroutine(Co_SpawnStage(stageData));
            }
            else
            {
                Debug.LogError($"[UnitSpawner] 스테이지 {stageIndex} 데이터를 찾을 수 없습니다!");
            }
        }

        private int GetLayerFromMask(LayerMask layerMask)
        {
            // LayerMask에서 첫 번째 활성화된 레이어 번호를 반환
            int layerNumber = 0;
            int mask = layerMask.value;
            
            while (mask > 1)
            {
                mask = mask >> 1;
                layerNumber++;
            }
            
            return layerNumber;
        }

        private void SpawnUnit(int unitLine, UnitSpawnData unitData)
        {
            if (unitLine < 0 || unitLine >= _unitLayerMasks.Length)
            {
                Debug.LogError($"[UnitSpawner] 잘못된 유닛 라인: {unitLine}");
                return;
            }

            if (_spawnPoint == null)
            {
                Debug.LogError($"[UnitSpawner] 스폰 포인트가 null입니다!");
                return;
            }

            if (unitData.UnitPrefab == null)
            {
                Debug.LogError($"[UnitSpawner] 유닛 프리팹이 null입니다!");
                return;
            }

            // 단일 스폰 포인트에서 생성
            GameObject spawnedUnit = Instantiate(unitData.UnitPrefab.gameObject, _spawnPoint.position, _spawnPoint.rotation);
            
            // 유닛에 해당 라인의 레이어 설정
            int unitLayer = GetLayerFromMask(_unitLayerMasks[unitLine]);
            spawnedUnit.layer = unitLayer;
            
            // 유닛 설정 적용
            UnitBase unitBase = spawnedUnit.GetComponent<UnitBase>();
            if (unitBase != null)
            {
                // 유닛 스탯 설정
                unitBase.SetStats(unitData);
                
                Debug.Log($"[UnitSpawner] 유닛 스폰 완료: {unitData.UnitPrefab.name} at Line {unitLine}, Layer: {unitLayer}");
            }
            else
            {
                Debug.LogWarning($"[UnitSpawner] 스폰된 오브젝트에 UnitBase 컴포넌트가 없습니다: {unitData.UnitPrefab.name}");
            }
        }

        // --------------------------------------------------
        // Methods - Coroutines
        // --------------------------------------------------
        private IEnumerator Co_SpawnStage(SoStage stageData)
        {
            Debug.Log($"[UnitSpawner] 스테이지 {stageData.StageIndex} 스폰 시작");

            try
            {
                // 각 유닛의 StartDelay에 따라 스폰 시작
                for (int i = 0; i < 100; i++) // 최대 100개 유닛까지 지원
                {
                    UnitSpawnData unitData = null;
                    
                    try
                    {
                        unitData = stageData.GetUnitData(i);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"[UnitSpawner] 인덱스 {i}에서 유닛 데이터 가져오기 실패: {e.Message}");
                        break;
                    }
                    
                    if (unitData?.UnitPrefab == null) 
                    {
                        Debug.Log($"[UnitSpawner] 인덱스 {i}에서 유닛 데이터 없음. 스폰 종료");
                        break;
                    }

                    int unitLine = i % 3; // 레이어 라인은 순환 (0,1,2,0,1,2...)
                    Debug.Log($"[UnitSpawner] 유닛 {i} 스폰 예약: {unitData.UnitPrefab.name}, Line: {unitLine}");
                    StartCoroutine(Co_SpawnUnitWithDelay(unitLine, unitData));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[UnitSpawner] 스테이지 스폰 중 오류 발생: {e.Message}");
            }

            Debug.Log($"[UnitSpawner] 스테이지 {stageData.StageIndex} 스폰 설정 완료");
            yield return null;
        }

        private IEnumerator Co_SpawnUnitWithDelay(int unitLine, UnitSpawnData unitData)
        {
            // StartDelay 대기
            if (unitData.StartDelay > 0f)
            {
                Debug.Log($"[UnitSpawner] {unitData.UnitPrefab.name} StartDelay 대기: {unitData.StartDelay}초, Line: {unitLine}");
                yield return new WaitForSeconds(unitData.StartDelay);
            }

            // 첫 번째 스폰
            SpawnUnit(unitLine, unitData);

            // SpawnDelay가 있으면 반복 스폰
            while (unitData.SpawnDelay > 0f)
            {
                yield return new WaitForSeconds(unitData.SpawnDelay);
                SpawnUnit(unitLine, unitData);
            }
        }
    }
}