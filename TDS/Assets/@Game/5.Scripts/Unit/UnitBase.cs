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
        [SerializeField] private Rigidbody2D _rigidbody2D = null;

        [Space(1.5f)]
        [Header("2. 유닛 옵션 그룹")]
        [SerializeField] private float _moveSpeed = 0f;
        [SerializeField] private float _attackPower = 0f;
        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private float _jumpForwardSpeed = 5f;

        [Space(1.5f)]
        [Header("3. 점프 옵션 그룹")]
        [SerializeField] private LayerMask _jumpTriggerLayer = -1;
        [SerializeField] private LayerMask _groundLayer = -1;
        [SerializeField] private float _jumpDuration = 1f;
        [SerializeField] private bool _canJump = true;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Const
        private const string ANIM_IDLE = "IsIdle";
        private const string ANIM_DEAD = "IsDead";
        private const string ANIM_ATTACK = "IsAttacking";
        private const string ANIM_JUMP = "IsJumping";

        // ----- Private
        private EUnitState _state = EUnitState.Unknown;
        private Coroutine _coUnitState = null;
        private bool _isGrounded = true;

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        private void Start()
        {
            if (_rigidbody2D == null)
                _rigidbody2D = GetComponent<Rigidbody2D>();
            
            if (_rigidbody2D == null)
                _rigidbody2D = gameObject.AddComponent<Rigidbody2D>();

            ChangeState(EUnitState.Move);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsLayerInMask(collision.gameObject.layer, _jumpTriggerLayer) && _canJump && _isGrounded 
                && transform.position.x > collision.transform.position.x && _state != EUnitState.Jump)
                ChangeState(EUnitState.Jump);

            if (IsLayerInMask(collision.gameObject.layer, _groundLayer))
                _isGrounded = true;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (IsLayerInMask(collision.gameObject.layer, _groundLayer))
                _isGrounded = true;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (IsLayerInMask(collision.gameObject.layer, _groundLayer))
                _isGrounded = false;
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

        private bool IsLayerInMask(int layer, LayerMask layerMask)
        {
            return (layerMask.value & (1 << layer)) != 0;
        }

        // --------------------------------------------------
        // Methods - Coroutines
        // --------------------------------------------------
        protected virtual IEnumerator Co_Move()
        {
            _animator.SetBool(ANIM_ATTACK, false);
            _animator.SetBool(ANIM_JUMP, false);
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
            _animator.SetBool(ANIM_JUMP, false);
            _animator.SetBool(ANIM_ATTACK, true);

            yield return null;
        }

        protected virtual IEnumerator Co_Jump()
        {
            _animator.SetBool(ANIM_IDLE, false);
            _animator.SetBool(ANIM_ATTACK, false);
            _animator.SetBool(ANIM_JUMP, true);

            if (_rigidbody2D != null)
            {
                var currentXVelocity = _rigidbody2D.velocity.x;
                var jumpXVelocity = currentXVelocity - _jumpForwardSpeed;
                _rigidbody2D.velocity = new Vector2(jumpXVelocity, _jumpPower);
            }

            _isGrounded = false;
            _canJump = false;

            yield return new WaitForSeconds(_jumpDuration);

            _canJump = true;
            var waitTime = 0f;
            while (!_isGrounded && waitTime < 0.25f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }

            if (!_isGrounded)
            {
                _isGrounded = true;
                Debug.LogWarning("점프 후 바닥 감지 실패 - 강제로 바닥 상태로 설정");
            }

            ChangeState(EUnitState.Move);
        }

        protected virtual IEnumerator Co_Dead()
        {
            _animator.SetBool(ANIM_IDLE, false);
            _animator.SetBool(ANIM_JUMP, false);
            _animator.SetBool(ANIM_DEAD, true);

            yield return null;
        }
    }
}