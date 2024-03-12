using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _jumpHeight;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _slideSpeed;

    [Header("Gravity Scale")] [SerializeField]
    private float _gravityMultiplier;

    private float _yVelocity;
    public float YVelocity => _yVelocity;

    private Camera _camera;

    private Vector3 _inputDirection;
    private Vector3 _velocity;

    private CharacterController _controller;

    private Vector3 _hitNormal;
    private bool _isSliding;
    private Vector3 _slideVelocity;

    private bool IsGrounded => _controller.isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;
    }

    private void Start()
    {
        Debug.Assert(_controller != null, "_controller !=null");

        // Cursor Visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected virtual void Update()
    {
        SetSlideVelocity();
        RotatePlayer();
        MovePlayer();
    }

    private void RotatePlayer()
    {
        Debug.Assert(_camera != null, "_camera != null");

        if (_inputDirection.sqrMagnitude == 0)
        {
            return;
        }

        Quaternion cameraRotation = _camera.transform.localRotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);
    }

    private void SetSlideVelocity()
    {
        float angle;
        Vector3 bottom = transform.position - new Vector3(0, _controller.height / 2, 0);
        if (Physics.Raycast(bottom, Vector3.down, out RaycastHit hit, 3.0f))
        {
            angle = Vector3.Angle(Vector3.up, hit.normal);

            if (angle > _controller.slopeLimit)
            {
                _slideVelocity = Vector3.ProjectOnPlane(new Vector3(0, _velocity.y, 0), hit.normal);
                _isSliding = true;
                return;
            }
        }

        const float TOLERANCE = 0.1f;
        angle = Vector3.Angle(Vector3.up, _hitNormal);
        if (angle > _controller.slopeLimit + TOLERANCE)
        {
            _slideVelocity = Vector3.ProjectOnPlane(new Vector3(0, _velocity.y, 0), _hitNormal);
            _isSliding = true;
            return;
        }

        _isSliding = false;
        _slideVelocity = Vector3.zero;
    }

    private void MovePlayer()
    {
        if (_isSliding && IsGrounded)
        {
            _velocity = _slideVelocity;
            _velocity.y += _yVelocity;
            _controller.Move(_slideSpeed * Time.deltaTime * _velocity);
            return;
        }

        _velocity = transform.TransformDirection(_inputDirection);

        ApplyGravity();

        _controller.Move(_moveSpeed * Time.deltaTime * _velocity);
    }

    private void ApplyGravity()
    {
        if (IsGrounded && _yVelocity < 0.0f)
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
        if (!context.started || !IsGrounded || _isSliding)
        {
            return;
        }

        PerformJump();
    }

    protected void PerformJump()
    {
        _yVelocity += _jumpHeight;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _hitNormal = hit.normal;
    }
}