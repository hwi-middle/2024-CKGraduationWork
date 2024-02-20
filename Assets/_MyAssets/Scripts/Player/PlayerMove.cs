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

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _moveDirection = Vector3.zero;

            _camera = Camera.main;
            
            // Cursor Invisible
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            MovePlayer();
            RotatePlayer();
        }

        private void MovePlayer()
        {
            _inputDirection.y += IsGround ? 0 : Physics.gravity.y * Time.fixedDeltaTime;
            _moveDirection = transform.TransformDirection(_inputDirection);
            
            _controller.Move(_moveDirection * (_moveSpeed * Time.fixedDeltaTime));
        }

        private void RotatePlayer()
        {
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
            
            _inputDirection.y = _jumpSpeed;
        }
    }
}
