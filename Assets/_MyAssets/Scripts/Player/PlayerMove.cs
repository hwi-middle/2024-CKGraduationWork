using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpSpeed;

        private Vector3 _inputDirection;

        private CharacterController _controller;
        private Vector3 _moveDirection;

        private Camera _camera;

        private bool IsGround
        {
            get
            {
                Debug.Assert(_controller != null, "_controller != null");
                return _controller.isGrounded;
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
            if (!context.performed || !IsGround)
            {
                return;
            }
            
            _moveDirection.y = _jumpSpeed;

            if (!HasInput)
            {
                _controller.Move(_moveDirection * (_moveSpeed * Time.fixedDeltaTime));
            }
        }
    }
}
