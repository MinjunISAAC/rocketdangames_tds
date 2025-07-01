// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;

namespace Game
{
    [CreateAssetMenu(fileName = "SoChapter", menuName = "Game/Created Chapter Data", order = 1)]
    public class SoChapter : ScriptableObject
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 챕터 정보")]
        [SerializeField] private int _chapterIndex = 0;
        [SerializeField] private List<SoStage> _stageList = new();

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private Dictionary<int, SoStage> _stageDictionary = new();

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public int ChapterIndex => _chapterIndex;

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public void Init()
        {
            _stageDictionary.Clear();

            foreach (var stage in _stageList)
            {
                if (stage != null)
                {
                    if (!_stageDictionary.ContainsKey(stage.StageIndex))
                    {
                        _stageDictionary.Add(stage.StageIndex, stage);
                        Debug.Log($"[SoChapter] 스테이지 {stage.StageIndex} 추가됨");
                    }
                    else
                    {
                        Debug.LogWarning($"[SoChapter] 스테이지 {stage.StageIndex}는 이미 존재합니다. 중복 스테이지를 무시합니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("[SoChapter] null 스테이지가 리스트에 포함되어 있습니다.");
                }
            }
            
            Debug.Log($"[SoChapter] 챕터 {_chapterIndex} 초기화 완료. 총 {_stageDictionary.Count}개 스테이지");
        }

        public List<SoStage> GetStageList()
        {
            return _stageList;
        }

        public SoStage GetStage(int stageIndex)
        {
            if (_stageDictionary.TryGetValue(stageIndex, out var stage))
                return stage;

            return null;
        }
    }
}