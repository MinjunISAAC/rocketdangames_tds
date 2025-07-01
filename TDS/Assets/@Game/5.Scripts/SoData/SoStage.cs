// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;

namespace Game
{
    [CreateAssetMenu(fileName = "SoStage", menuName = "Game/Created Stage Data", order = 2)]
    public class SoStage : ScriptableObject
    {
        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        [SerializeField] private int _stageIndex = 0;
        [SerializeField] private List<UnitSpawnData> _unitDataList = new();

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public int StageIndex => _stageIndex;

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public UnitSpawnData GetUnitData(int index)
        {
            if (index < 0 || index >= _unitDataList.Count)
                return null;

            return _unitDataList[index];
        }
    }
}