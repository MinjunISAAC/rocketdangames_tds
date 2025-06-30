// ----- System
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace Game
{
    public class UnitBase : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("1. 애니메이션 그룹")]
        [SerializeField] private Animator _animator = null;


        [Space(1.5f)]
        [Header("2. 유닛 옵션 그룹")]
        [SerializeField] private float _moveSpeed = 0f;
        [SerializeField] private float _attackPower = 0f;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Const
        private const string ANIM_IDLE = "IsIdle";
        private const string ANIM_DEAD = "IsDead";
        private const string ANIM_ATTACK = "IsAttacking";

        private EUnitState _state = EUnitState.Unknown;
        private Coroutine _coUnitState = null;

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        private void Start()
        {
            ChangeState(EUnitState.Move);
        }


        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public void Spawn(int startLayerCount)
        {
            
        }

        public void ChangeState(EUnitState state)
        {
            if (_coUnitState != null)
                StopCoroutine(_coUnitState);

            _state = state;

            switch(state)
            {
                case EUnitState.Move:
                    _coUnitState = StartCoroutine(Co_Move());
                    break;
                case EUnitState.Attack:
                    _coUnitState = StartCoroutine(Co_Attack());
                    break;
                case EUnitState.Jump:
                    _coUnitState = StartCoroutine(Co_Jump());
                    break;
                case EUnitState.Dead:
                    _coUnitState = StartCoroutine(Co_Dead());
                    break;
            }
        }

        // --------------------------------------------------
        // Methods - Coroutines
        // --------------------------------------------------
        protected virtual IEnumerator Co_Move()
        {
            _animator.SetBool(ANIM_ATTACK, false);
            _animator.SetBool(ANIM_IDLE, true);

            while(_state == EUnitState.Move)
            {
                transform.position += Vector3.left * _moveSpeed * Time.deltaTime;
                yield return null;
            }
        }

        protected virtual IEnumerator Co_Attack()
        {
            _animator.SetBool(ANIM_IDLE, false);
            _animator.SetBool(ANIM_ATTACK, true);

            yield return null;
        }

        protected virtual IEnumerator Co_Jump()
        {
            _animator.SetBool(ANIM_IDLE, false);
            _animator.SetBool(ANIM_ATTACK, true);

            yield return null;
        }

        protected virtual IEnumerator Co_Dead()
        {
            _animator.SetBool(ANIM_IDLE, false);
            _animator.SetBool(ANIM_DEAD, true);

            yield return null;
        }
    }
}