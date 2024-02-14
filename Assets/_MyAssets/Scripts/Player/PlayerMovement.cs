using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpSpeed;
        
        private Vector3 _moveDirection;

        private Rigidbody _rigidbody;

        private static readonly float RAY_DISTANCE = 1f;
        
        private bool CanJump
        {
            get
            {
                return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, RAY_DISTANCE);
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
            if (_moveDirection == Vector3.zero || !CanJump)
            {
                return;
            }

            Debug.Assert(_camera != null, "Camera.main != null");

            Quaternion cameraRotation = _camera.transform.localRotation;
            cameraRotation.x = 0;
            cameraRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);

            Vector3 worldMoveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);
            Vector3 finalMoveDirection = _camera.transform.TransformDirection(worldMoveDirection);

            _rigidbody.velocity = new Vector3(finalMoveDirection.x * _moveSpeed, _rigidbody.velocity.y,
                finalMoveDirection.z * _moveSpeed);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            _moveDirection = new Vector3(input.x, 0f, input.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed || !CanJump)
            {
                return;
            }

            _rigidbody.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
        }
    }
}