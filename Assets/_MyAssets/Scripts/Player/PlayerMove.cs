using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _slideSpeed;

        private Camera _camera;
        
        private Vector3 _inputDirection;

        private CharacterController _controller;
        private Vector3 _moveDirection;
        private Vector3 _hitNormal;

        
        private bool IsGround
        {
            get
            {
                Debug.Assert(_controller != null, "_controller != null");
                
                return _controller.isGrounded;
            }
        }

        private bool IsLimitSlope
        {
            get
            {
                Debug.Assert(_controller != null, "_controller != null");

                float angle = Vector3.Angle(Vector3.up, _hitNormal);
                return angle <= _controller.slopeLimit && angle != 0;
            }
        }

        private bool HasInput
        {
            get
            {
                return _inputDirection != Vector3.zero;
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _moveDirection = Vector3.zero;

            _camera = Camera.main;
            
            // Cursor Invisible
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FixedUpdate()
        {
            RotatePlayer();
            MovePlayer();
        }

        private void MovePlayer()
        {
            // 가파른 경사로에서는 미끄러짐
            if (IsLimitSlope && IsGround)
            {
                _moveDirection.x += (1f - _hitNormal.y) * _hitNormal.x;
                _moveDirection.z += (1f - _hitNormal.y) * _hitNormal.z;
                _controller.Move(_moveDirection * (_slideSpeed * Time.fixedDeltaTime));
                return;
            }
            
            // 입력이 아무것도 없을 때 공중에 있지 않으면 연산하지 않음
            if (!HasInput && IsGround)
            {
                return;
            }
            
            _inputDirection.y = _moveDirection.y;
            _moveDirection = transform.TransformDirection(_inputDirection);
            
            _moveDirection.y += Physics.gravity.y * Time.fixedDeltaTime;
            _controller.Move(_moveDirection * (_moveSpeed * Time.fixedDeltaTime));
        }

        private void RotatePlayer()
        {
            if (!HasInput)
            {
                return;
            }
            
            Debug.Assert(_camera != null, "_camera != null");
            
            Quaternion cameraRotation = _camera.transform.localRotation;
            cameraRotation.x = 0;
            cameraRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            _inputDirection = new Vector3(input.x, 0, input.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed || !IsGround || IsLimitSlope)
            {
                return;
            }
            
            _moveDirection.y = _jumpSpeed;

            if (!HasInput)
            {
                _controller.Move(_moveDirection * (_moveSpeed * Time.fixedDeltaTime));
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _hitNormal = hit.normal;
        }
    }
}
