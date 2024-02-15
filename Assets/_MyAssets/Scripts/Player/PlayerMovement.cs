using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _rayDistance = 1f;
        
        private Vector3 _moveDirection;
        private Vector3 _finalMoveDirection;

        private Rigidbody _rigidbody;
        
        private bool IsGround
        {
            get
            {
                return Physics.Raycast(transform.position, Vector3.down, _rayDistance);
            }
        }

        private bool IsWall
        {
            get
            {
                Vector3 myPos = transform.position;
                Vector3 firstDirection = new Vector3(_finalMoveDirection.x, 0, 0);
                Vector3 secondDirection = new Vector3(0, 0, _finalMoveDirection.z);

                bool isWall = Physics.Raycast(myPos, _finalMoveDirection, _rayDistance * 0.55f)
                              || Physics.Raycast(myPos, firstDirection, _rayDistance * 0.55f)
                              || Physics.Raycast(myPos, secondDirection, _rayDistance * 0.55f);

                return isWall;
            }
        }

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            _rigidbody = gameObject.GetComponent<Rigidbody>();

            // Cursor invisible
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (_moveDirection == Vector3.zero)
            {
                return;
            }

            Debug.Assert(_camera != null, "Camera.main != null");

            Quaternion cameraRotation = _camera.transform.localRotation;
            cameraRotation.x = 0;
            cameraRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);

            Vector3 worldMoveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);
            _finalMoveDirection = _camera.transform.TransformDirection(worldMoveDirection);

            if (IsWall)
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
                return;
            }
            
            _rigidbody.velocity = new Vector3(_finalMoveDirection.x * _moveSpeed, 
                _rigidbody.velocity.y,
                _finalMoveDirection.z * _moveSpeed);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            _moveDirection = new Vector3(input.x, 0, input.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed || !IsGround)
            {
                return;
            }

            _rigidbody.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
        }
    }
}