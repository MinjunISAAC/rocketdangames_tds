// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

// ----- User Defined
using Core;

namespace Game
{
    [CreateAssetMenu(fileName = "New Unit Group", menuName = "Game/Unit Group Data", order = 3)]
    public class SoUnitGroup : ScriptableObject
    {
        [Header("1. 유닛 그룹 정보")]
        [SerializeField] private float _maxHp = 0;
        [SerializeField] private float _moveSpeed = 0;
        [SerializeField] private float _attackRange = 0;
        [SerializeField] private float _attackDelay = 0;
        [SerializeField] private float _attackPower = 0;
        [SerializeField] private float _attackSpeed = 0;
    }
}