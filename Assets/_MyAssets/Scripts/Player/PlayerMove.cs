using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _slideSpeed;


        [Header("Gravity Scale")]
        [SerializeField] private float _gravityMultiplier;
        private float _yVelocity;
        
        private Camera _camera;
        
        private Vector3 _inputDirection;
        private Vector3 _velocity;

        private CharacterController _controller;
        private Vector3 _hitNormal;

        private bool IsGround => _controller.isGrounded;

        private bool IsLimitSlope
        {
            get
            {
                float angle = Vector3.Angle(Vector3.up, _hitNormal);
                return angle >= _controller.slopeLimit && angle <= 90 && IsGround;
            }
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _camera = Camera.main;
        }

        private void Start()
        {
            // Cursor Visible
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (IsLimitSlope)
            {
                SlidePlayer();
                return;
            }

            if (_inputDirection.sqrMagnitude == 0 && IsGround)
            {
                return;
            }
            
            RotatePlayer();
            MovePlayer();
        }

        private void RotatePlayer()
        {
            Debug.Assert(_camera != null, "_camera != null");
            
            Quaternion cameraRotation = _camera.transform.localRotation;
            cameraRotation.x = 0;
            cameraRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);
        }

        private void SlidePlayer()
        {
            float yInverse = 1f - _hitNormal.y;
            _velocity.x += yInverse * _hitNormal.x;
            _velocity.z += yInverse * _hitNormal.z;
            _controller.Move(_velocity * (_slideSpeed * Time.deltaTime));
        }

        private void MovePlayer()
        {
            _velocity = transform.TransformDirection(_inputDirection);
            
            GravityCalculate();
            
            _controller.Move(_velocity * (_moveSpeed * Time.deltaTime));
        }

        private void GravityCalculate()
        {
            if (IsGround && _yVelocity < 0.0f)
            {
                _yVelocity = -1.0f;
            }
            else
            {
                _yVelocity += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            }
            
            _velocity.y = _yVelocity;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            _inputDirection = new Vector3(input.x, 0, input.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.started || !IsGround || IsLimitSlope)
            {
                return;
            }


            if (_inputDirection.sqrMagnitude != 0)
            {

                _yVelocity += _jumpPower;
                return;
            }

            _yVelocity = 0;
            _yVelocity += _jumpPower * 0.8f;
            MovePlayer();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _hitNormal = hit.normal;
        }
    }
}
