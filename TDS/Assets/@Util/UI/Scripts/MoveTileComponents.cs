// ----- Unity
using UnityEngine;
using UnityEngine.UI;

// ----- User Defined
using Core;

namespace UI
{
    public class UI_MoveTileComponent : UI_Base
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [SerializeField] private Define.ETileMoveType _moveType = Define.ETileMoveType.LeftTop_RightBottom;
        [SerializeField] private float _moveSpeed = 0.1f;
        [SerializeField] private Image _IMG_Move = null;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private Material _material = null;

        // --------------------------------------------------
        // Functions - Event
        // --------------------------------------------------
        private void Start()
        {
            if (_IMG_Move != null)
            {
                _material = _IMG_Move.material;
                _material.SetTextureOffset("_MainTex", Vector2.zero);
            }
        }

        private void Update()
        {
            if (_IMG_Move.materialForRendering != _IMG_Move.material)
                _material = _IMG_Move.materialForRendering;

            float offset = Time.time * _moveSpeed;

            Vector2 moveOffset;

            switch (_moveType)
            {
                case Define.ETileMoveType.LeftBottom_RightTop: moveOffset = new Vector2(-offset, -offset); break;
                case Define.ETileMoveType.LeftTop_RightBottom: moveOffset = new Vector2(-offset, offset); break;
                case Define.ETileMoveType.RightBottom_LeftTop: moveOffset = new Vector2(offset, -offset); break;
                case Define.ETileMoveType.RightTop_LeftBottom: moveOffset = new Vector2(offset, offset); break;
                case Define.ETileMoveType.Left_Right: moveOffset = new Vector2(-offset, 0); break;
                case Define.ETileMoveType.Right_Left: moveOffset = new Vector2(offset, 0); break;
                case Define.ETileMoveType.Top_Bottom: moveOffset = new Vector2(0, offset); break;
                case Define.ETileMoveType.Bottom_Top: moveOffset = new Vector2(0, -offset); break;
                default: moveOffset = new Vector2(-offset, -offset); break;
            }

            _material.SetTextureOffset("_MainTex", moveOffset);
        }
    }
}