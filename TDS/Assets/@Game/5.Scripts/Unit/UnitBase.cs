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
        [Header("1. 움직임 그룹")]
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Rigidbody2D _rigidbody2D = null;

        [Space(1.5f)]
        [Header("2. 유닛 옵션 그룹")]
        [SerializeField] private float _maxHp = 0f;
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

        [Space(1.5f)]
        [Header("4. 공격 옵션 그룹")]
        [SerializeField] private float _attackDelay = 0.5f;

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
        private BoxBase _currentTargetBox = null;
        private float _currentHp = 0f;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public float CurrentHp => _currentHp;
        public float MaxHp => _maxHp;
        public bool IsAlive => _currentHp > 0f;
        public float MoveSpeed => _moveSpeed;
        public float AttackPower => _attackPower;

        // --------------------------------------------------
        // Methods - Event
        // --------------------------------------------------
        private void Start()
        {
            if (_rigidbody2D == null)
                _rigidbody2D = GetComponent<Rigidbody2D>();
            
            if (_rigidbody2D == null)
                _rigidbody2D = gameObject.AddComponent<Rigidbody2D>();

            // HP 초기화 (SetStats가 호출되지 않은 경우를 대비)
            if (_currentHp <= 0f)
                _currentHp = _maxHp;

            ChangeState(EUnitState.Move);
        }

        private void Update()
        {
            // 디버깅용 - 스페이스바로 점프 테스트
            if (Input.GetKeyDown(KeyCode.Space) && _canJump && _isGrounded && _state != EUnitState.Jump)
            {
                Debug.Log($"[{gameObject.name}] 키보드 점프 테스트!");
                ChangeState(EUnitState.Jump);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 점프 트리거 체크 (디버깅 로그 추가)
            bool isJumpTriggerLayer = IsLayerInMask(collision.gameObject.layer, _jumpTriggerLayer);
            bool isRightPosition = transform.position.x > collision.transform.position.x;
            bool isNotJumping = _state != EUnitState.Jump;
            
            Debug.Log($"[{gameObject.name}] 충돌 감지 - 객체: {collision.gameObject.name}, 레이어: {collision.gameObject.layer}");
            Debug.Log($"[{gameObject.name}] 점프 조건 체크 - 트리거레이어: {isJumpTriggerLayer}, 점프가능: {_canJump}, 바닥접촉: {_isGrounded}, 위치조건: {isRightPosition}, 점프중아님: {isNotJumping}");
            
            if (isJumpTriggerLayer && _canJump && _isGrounded && isRightPosition && isNotJumping)
            {
                Debug.Log($"[{gameObject.name}] 점프 실행!");
                ChangeState(EUnitState.Jump);
            }

            // 바닥 감지
            if (IsLayerInMask(collision.gameObject.layer, _groundLayer))
                _isGrounded = true;

            // 박스 충돌 감지 (BoxBase 컴포넌트로 직접 확인)
            BoxBase boxBase = collision.gameObject.GetComponent<BoxBase>();
            if (boxBase != null && _state != EUnitState.Attack)
            {
                Debug.Log($"[{gameObject.name}] 박스와 충돌 감지!");
                OnBoxCollision(boxBase);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (IsLayerInMask(collision.gameObject.layer, _groundLayer))
                _isGrounded = true;

            // 박스와 계속 접촉 중인지 확인
            BoxBase boxBase = collision.gameObject.GetComponent<BoxBase>();
            if (boxBase != null)
            {
                _currentTargetBox = boxBase;
                
                // 공격 상태가 아니면 공격 시작
                if (_state != EUnitState.Attack)
                {
                    Debug.Log($"[{gameObject.name}] 박스와 계속 접촉 중 - 공격 재시작!");
                    OnBoxCollision(boxBase);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (IsLayerInMask(collision.gameObject.layer, _groundLayer))
                _isGrounded = false;

            // 박스와의 접촉이 끝났는지 확인
            BoxBase boxBase = collision.gameObject.GetComponent<BoxBase>();
            if (boxBase != null && _currentTargetBox == boxBase)
            {
                Debug.Log($"[{gameObject.name}] 박스와 접촉 종료");
                _currentTargetBox = null;
            }
        }

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public void Spawn(int startLine, int groundLayer)
        {
            
        }

        public void SetStats(UnitSpawnData unitData)
        {
            if (unitData == null)
            {
                Debug.LogWarning($"[{gameObject.name}] UnitSpawnData가 null입니다!");
                return;
            }

            // 스탯 설정
            _maxHp = unitData.MaxHp;
            _moveSpeed = unitData.MoveSpeed;
            _attackPower = unitData.AttackPower;
            _attackDelay = unitData.AttackDelay;
            
            // 점프 관련 설정
            _jumpPower = unitData.JumpPower;
            _jumpForwardSpeed = unitData.JumpForwardSpeed;
            _jumpDuration = unitData.JumpDuration;
            
            // HP 초기화
            _currentHp = _maxHp;
            
            Debug.Log($"[{gameObject.name}] 스탯 설정 완료 - HP: {_maxHp}, 이동속도: {_moveSpeed}, 공격력: {_attackPower}");
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

        private void OnBoxCollision(BoxBase boxBase)
        {
            Debug.Log($"[{gameObject.name}] 박스 공격 시작! 데미지: {_attackPower}");
            
            // 공격 상태로 변경
            ChangeState(EUnitState.Attack);
            
            // 박스에 데미지 주기
            boxBase.Hit(_attackPower);
        }

        public void Hit(float damage)
        {
            if (!IsAlive) return;

            _currentHp -= damage;
            Debug.Log($"[{gameObject.name}] Hit! -{damage} damage. HP: {_currentHp:F1}/{_maxHp:F1}");

            if (_currentHp <= 0f)
            {
                OnUnitDeath();
            }
        }

        private void OnUnitDeath()
        {
            Debug.Log($"[{gameObject.name}] 유닛 사망!");
            
            // 죽음 상태로 변경
            ChangeState(EUnitState.Dead);
            
            // 일정 시간 후 오브젝트 파괴 (애니메이션 시간을 고려)
            Destroy(gameObject, 2f);
        }

        // --------------------------------------------------
        // Methods - Coroutines
        // --------------------------------------------------
        protected virtual IEnumerator Co_Move()
        {
            _animator.SetBool(ANIM_ATTACK, false);
            _animator.SetBool(ANIM_JUMP, false);
            _animator.SetBool(ANIM_IDLE, true);

            while(_state == EUnitState.Move && IsAlive)
            {
                transform.position += Vector3.left * _moveSpeed * Time.deltaTime;
                yield return null;
            }

            // 죽었으면 Dead 상태로 변경
            if (!IsAlive && _state != EUnitState.Dead)
            {
                ChangeState(EUnitState.Dead);
            }
        }

        protected virtual IEnumerator Co_Attack()
        {
            _animator.SetBool(ANIM_IDLE, false);
            _animator.SetBool(ANIM_JUMP, false);
            _animator.SetBool(ANIM_ATTACK, true);

            yield return new WaitForSeconds(_attackDelay);

            // 죽었으면 Dead 상태로 변경
            if (!IsAlive)
            {
                ChangeState(EUnitState.Dead);
                yield break;
            }

            if (_currentTargetBox != null && _currentTargetBox.IsAlive)
            {
                _currentTargetBox.Hit(_attackPower);
                _coUnitState = StartCoroutine(Co_Attack());
            }
            else
            {
                Debug.Log($"[{gameObject.name}] 공격 완료, 이동 상태로 복귀");
                ChangeState(EUnitState.Move);
            }
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
            _animator.SetBool(ANIM_ATTACK, false);
            _animator.SetBool(ANIM_DEAD, true);

            // 물리 비활성화 (더 이상 움직이지 않도록)
            if (_rigidbody2D != null)
            {
                _rigidbody2D.velocity = Vector2.zero;
                _rigidbody2D.isKinematic = true;
            }

            // 죽음 상태에서는 아무것도 하지 않음
            while (_state == EUnitState.Dead)
            {
                yield return null;
            }
        }
    }
}