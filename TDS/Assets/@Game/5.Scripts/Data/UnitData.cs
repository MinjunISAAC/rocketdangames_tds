// ----- System
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace Game
{
    [Serializable]
    public class UnitSpawnData
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [SerializeField] private UnitBase _unitPrefab = null;
        [SerializeField] private float _maxHp = 0;
        [SerializeField] private float _moveSpeed = 0;
        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private float _jumpForwardSpeed = 5f;
        [SerializeField] private float _jumpDuration = 1f;
        [SerializeField] private float _attackDelay = 0;
        [SerializeField] private float _attackPower = 0;
        [SerializeField] private float _startDelay = 0;
        [SerializeField] private float _spawnDelay = 0;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public UnitBase UnitPrefab => _unitPrefab;
        public float MaxHp => _maxHp;
        public float MoveSpeed => _moveSpeed;
        public float JumpPower => _jumpPower;
        public float JumpForwardSpeed => _jumpForwardSpeed;
        public float JumpDuration => _jumpDuration;
        public float AttackDelay => _attackDelay;
        public float AttackPower => _attackPower;
        public float StartDelay => _startDelay;
        public float SpawnDelay => _spawnDelay;
        
        // 에디터에서 스프라이트 미리보기용
        public Sprite GetPreviewSprite()
        {
            if (_unitPrefab != null)
            {
                SpriteRenderer spriteRenderer = _unitPrefab.GetComponent<SpriteRenderer>();
                return spriteRenderer != null ? spriteRenderer.sprite : null;
            }
            return null;
        }
    }
}