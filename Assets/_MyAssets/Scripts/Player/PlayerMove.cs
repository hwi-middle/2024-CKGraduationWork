using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Flags]
public enum EPlayerState
{
    None = 0,
    Idle = 1 << 1,
    Walk = 1 << 2,
    Run = 1 << 3,
    Crouch = 1 << 4,
    Hide = 1 << 5,
    Peek = 1 << 6,
    Alive = 1 << 7,
    Dead = 1 << 8,
    Overstep = 1 << 9,
}

public class PlayerMove : Singleton<PlayerMove>
{
    private bool _isInitialized = false;

    private Camera _camera;

    [SerializeField] private PlayerInputData _inputData;
    private int _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;

    [Header("Player Data")] [SerializeField]
    private PlayerData _playerData;

    public GameObject PlayerCanvas => _playerCanvas;
    private GameObject _playerCanvas;

    private Vector3 _targetPosition;
    private float _playerApplySpeed;

    // Player Respawn
    private IEnumerator _movePlayerToRespawnPointRoutine;
    [SerializeField] private float _respawnMoveDuration;

    // Noise
    private MakeNoiseHandler _makeNoiseHandler;

    [SerializeField] private PlayerAssassinationData _assassinationData;

    private bool _isAssassinating = false;

    [Header("Gravity Scale")] [SerializeField]
    private float _gravityMultiplier;

    private float _yVelocity;

    private Vector3 _inputDirection;
    private Vector3 _velocity;

    private CharacterController _controller;

    private GameObject _hitObject;
    private Vector3 _hitNormal;
    private bool _isSliding;
    private Vector3 _slideVelocity;

    private bool CanActing => !CheckPlayerState(EPlayerState.Dead) && !CheckPlayerState(EPlayerState.Overstep);

    private bool IsGrounded => _controller.isGrounded;

    protected virtual void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerCanvas = Instantiate(_playerData.playerCanvas);

        Instantiate(_playerData.lineRendererPrefab);
        LineDrawHelper.Instance.DisableLine();

        _makeNoiseHandler = GetComponent<MakeNoiseHandler>();
    }

    private void OnEnable()
    {
        _inputData.moveEvent += HandleMoveAction;
        _inputData.runEvent += HandleRunAction;
        _inputData.runQuitEvent += HandleQuitRunAction;
        _inputData.crouchEvent += HandleCrouchAction;
    }

    private void OnDisable()
    {
        _inputData.moveEvent -= HandleMoveAction;
        _inputData.runEvent -= HandleRunAction;
        _inputData.runQuitEvent -= HandleQuitRunAction;
        _inputData.crouchEvent -= HandleCrouchAction;
    }

    private void Start()
    {
        Debug.Assert(_controller != null, "_controller !=null");

        _camera = Camera.main;
    }

    protected virtual void Update()
    {
        if (CanActing)
        {
            RotatePlayer();
            MovePlayer();
        }

        if (!_isInitialized)
        {
            Vector3 checkPoint = CheckPointRootHandler.Instance.LastCheckPoint;
            if (checkPoint != Vector3.zero)
            {
                _controller.enabled = false;
                transform.position = checkPoint;
                _isInitialized = true;
                _controller.enabled = true;
            }
            else
            {
                _isInitialized = true;
            }
        }

        MakeNoiseByCurrentState();
    }

    private void MakeNoiseByCurrentState()
    {
        if (CheckPlayerState(EPlayerState.Run))
        {
            float noiseRadius = _playerData.sprintNoiseRadius;
            float noiseIncrement = _playerData.sprintNoiseIncrementPerSecond;
            _makeNoiseHandler.OnMakeNoise(noiseRadius, noiseIncrement * Time.deltaTime);
        }
        else if (CheckPlayerState(EPlayerState.Walk))
        {
            bool isCrouch = CheckPlayerState(EPlayerState.Crouch);
            float noiseRadius = isCrouch ? _playerData.crouchWalkNoiseRadius : _playerData.walkNoiseRadius;
            float noiseIncrement = isCrouch ? _playerData.crouchWalkNoiseIncrementPerSecond : _playerData.walkNoiseIncrementPerSecond;
            _makeNoiseHandler.OnMakeNoise(noiseRadius, noiseIncrement * Time.deltaTime);
        }
    }

    public void AlignPlayerToCameraForward()
    {
        if (CameraController.Instance.IsBlending)
        {
            return;
        }

        ApplyRotate();
    }

    private void RotatePlayer()
    {
        Debug.Assert(_camera != null, "_camera != null");

        if (_inputDirection.sqrMagnitude == 0)
        {
            return;
        }

        ApplyRotate();
    }

    private void ApplyRotate()
    {
        Quaternion cameraRotation = _camera.transform.localRotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation =
            Quaternion.Slerp(transform.rotation, cameraRotation, _playerData.rotateSpeed * Time.deltaTime);
    }

    private void MovePlayer()
    {
        _velocity = transform.TransformDirection(_inputDirection);

        ApplyGravity();
        ApplyPlayerMoveSpeed();

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void ApplyPlayerMoveSpeed()
    {
        if (CheckPlayerState(EPlayerState.Run))
        {
            _playerApplySpeed = _playerData.runSpeed;
        }
        else if (CheckPlayerState(EPlayerState.Crouch))
        {
            _playerApplySpeed = _playerData.crouchSpeed;
        }
        else
        {
            _playerApplySpeed = _playerData.walkSpeed;
        }

        _velocity.x *= _playerApplySpeed;
        _velocity.z *= _playerApplySpeed;
        _velocity.y *= _playerData.yMultiplier;
    }

    private void ApplyGravity()
    {
        if (IsGrounded && _yVelocity < 0.0f && !_isSliding)
        {
            _yVelocity = -1.0f;
        }
        else
        {
            _yVelocity += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
        }

        _velocity.y = _yVelocity;
    }

    // Animation 관리를 위해 Public
    public bool CheckPlayerState(EPlayerState state)
    {
        return (_currentState & (int)state) != 0;
    }

    public void SetInitState()
    {
        _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;
    }

    public void SetDeadState()
    {
        _currentState = (int)EPlayerState.Dead;
    }

    public void ExitHideState(bool isCrouch)
    {
        RemovePlayerState(EPlayerState.Hide);
        if (!isCrouch)
        {
            return;
        }

        AddPlayerState(EPlayerState.Crouch);
    }

    public void AddPlayerState(EPlayerState state)
    {
        _currentState |= (int)state;
    }

    public void RemovePlayerState(EPlayerState state)
    {
        _currentState &= ~(int)state;
    }

    private void HandleMoveAction(Vector2 pos)
    {
        _inputDirection = new Vector3(pos.x, 0, pos.y);

        if (_inputDirection.sqrMagnitude == 0)
        {
            RemovePlayerState(EPlayerState.Walk);
            return;
        }

        AddPlayerState(EPlayerState.Walk);
    }

    private void HandleRunAction()
    {
        if (CameraController.Instance.IsOnChangeHeightRoutine || !CheckPlayerState(EPlayerState.Walk))
        {
            return;
        }

        // Run과 Crouch 상태 중 우선 순위는 Run
        if (CheckPlayerState(EPlayerState.Crouch))
        {
            RemovePlayerState(EPlayerState.Crouch);
            CameraController.Instance.ToggleCrouchCameraHeight(false);
        }

        AddPlayerState(EPlayerState.Run);
    }

    private void HandleQuitRunAction()
    {
        RemovePlayerState(EPlayerState.Run);
    }

    private void HandleCrouchAction()
    {
        if (!IsGrounded || CheckPlayerState(EPlayerState.Run) || CameraController.Instance.IsOnChangeHeightRoutine
            || !CanActing)
        {
            return;
        }

        if (CheckPlayerState(EPlayerState.Crouch))
        {
            CameraController.Instance.ToggleCrouchCameraHeight(false);
            RemovePlayerState(EPlayerState.Crouch);
            return;
        }

        CameraController.Instance.ToggleCrouchCameraHeight(true);
        AddPlayerState(EPlayerState.Crouch);
    }
}
