using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 _moveDirection;

        private static readonly float MOVE_SPEED = 5.0f;
        private static readonly float JUMP_SPEED = 5.0f;

        private Rigidbody _rigidbody;

        private bool _canJump;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            _rigidbody = gameObject.GetComponent<Rigidbody>();

            _canJump = true;

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

            transform.Translate(_moveDirection.normalized * (MOVE_SPEED * Time.deltaTime));
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            _moveDirection = new Vector3(input.x, 0f, input.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed || !_canJump)
            {
                return;
            }

            _canJump = false;
            _rigidbody.AddForce(Vector3.up * JUMP_SPEED, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision coll)
        {
            if (coll.transform.CompareTag("Ground"))
            {
                _canJump = true;
            }
        }
    }
}