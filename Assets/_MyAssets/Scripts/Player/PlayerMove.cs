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
    Hide = 1 << 7,
    Peek = 1 << 8,
    Alive = 1 << 9,
    Dead = 1 << 10,
}

public class PlayerMove : Singleton<PlayerMove>
{
    private bool _isInitialized = false;
    
    private Camera _camera;
    
    [SerializeField] private PlayerInputData _inputData;
    private int _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;
    
    [Header("Player Data")]
    [SerializeField] private PlayerData _playerData;

    public GameObject PlayerCanvas => _playerCanvas;
    private GameObject _playerCanvas;

    private Vector3 _targetPosition;

    private float _playerApplySpeed;

    private static readonly Vector3 CAMERA_CENTER_POINT = new(0.5f, 0.5f, 0.0f);
    
    // Player Respawn
    private IEnumerator _movePlayerToRespawnPointRoutine;
    [SerializeField] private float _respawnMoveDuration;
    
    // Noise
    private MakeNoiseHandler _makeNoiseHandler;

    private enum EAssassinationType
    {
        Ground, // 평지에서 암살
        Fall, // 위에서 아래로 떨어지며 암살
        Jump, // 아래에서 위로 점프하며 암살
    }

    [SerializeField] private PlayerAssassinationData _assassinationData;

    private bool _isAssassinating = false;
    private Transform _assassinationTarget;

    [Header("Gravity Scale")] [SerializeField]
    private float _gravityMultiplier;

    private float _yVelocity;
    private float YVelocity => _yVelocity;

    private Vector3 _inputDirection;
    private Vector3 _velocity;

    private CharacterController _controller;

    private GameObject _hitObject;
    private Vector3 _hitNormal;
    private bool _isSliding;
    private Vector3 _slideVelocity;

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

        // Cursor Visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _camera = Camera.main;
    }

    protected virtual void Update()
    {
        if (!CheckPlayerState(EPlayerState.Dead))
        {
            RotatePlayer();
            MovePlayer();
        }

        if (!_isInitialized)
        {
            Vector3 checkPoint = CheckPointRootHandler.Instance.LastCheckPoint;
            if(checkPoint != Vector3.zero)
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
        
        if (!_isAssassinating)
        {
            _assassinationTarget = GetAimingEnemy();
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
            _makeNoiseHandler.OnMakeNoise(_playerData.sprintNoiseRadius, _playerData.sprintNoiseIncrementPerSecond * Time.deltaTime);
        }
        else if (CheckPlayerState(EPlayerState.Crouch))
        {
            _playerApplySpeed = _playerData.crouchSpeed;
            _makeNoiseHandler.OnMakeNoise(_playerData.crouchWalkNoiseRadius, _playerData.crouchWalkNoiseIncrementPerSecond * Time.deltaTime);
        }
        else
        {
            _playerApplySpeed = _playerData.walkSpeed;
            _makeNoiseHandler.OnMakeNoise(_playerData.walkNoiseRadius, _playerData.walkNoiseIncrementPerSecond * Time.deltaTime);
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

    private List<GameObject> GetWirePoints()
    {
        List<GameObject> detectedWirePoints = new();
        Vector3 playerPos = transform.position;

        Collider[] wirePointsInRange =
            Physics.OverlapSphere(playerPos, _playerData.maxWireDistance, LayerMask.GetMask("WirePoint"));

        foreach (Collider wirePoint in wirePointsInRange)
        {
            Vector3 wirePointPos = wirePoint.transform.position;
            float distance = (wirePointPos - playerPos).magnitude;
            if (distance < _playerData.minWireDistance || _camera.WorldToViewportPoint(wirePointPos).z < 0)
            {
                continue;
            }

            detectedWirePoints.Add(wirePoint.gameObject);
        }

        return detectedWirePoints;
    }

    private GameObject FindNearestWirePointFromAim()
    {
        List<GameObject> wirePointInScreen = GetWirePoints();

        if (wirePointInScreen.Count == 0)
        {
            return null;
        }

        const float INFINITY = 2.0f;
        float minDistanceFromCenter = INFINITY;
        GameObject nearestWirePoint = null;

        foreach (GameObject wirePoint in wirePointInScreen)
        {
            Vector3 wirePointPos = wirePoint.transform.position;
            Vector3 viewportPos = _camera.WorldToViewportPoint(wirePointPos);

            float distanceFromAim = ((Vector2)CAMERA_CENTER_POINT - (Vector2)viewportPos).magnitude;
            const float RESTRICT_RANGE_FROM_CENTER = 0.15f;

            if (distanceFromAim > minDistanceFromCenter || distanceFromAim > RESTRICT_RANGE_FROM_CENTER)
            {
                continue;
            }

            minDistanceFromCenter = distanceFromAim;
            nearestWirePoint = wirePoint;
        }

        return nearestWirePoint;
    }
    
    private Transform GetAimingEnemy()
    {
        Camera mainCamera = Camera.main;
        Debug.Assert(mainCamera != null);
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        float assassinateDistance = _assassinationData.assassinateDistance;
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("BypassAiming"));

        if (Physics.Raycast(ray, out RaycastHit hit, assassinateDistance, layerMask) &&
            hit.transform.CompareTag("AssassinationTarget"))
        {
            return hit.transform;
        }

        return null;
    }

    private void Assassinate()
    {
        Debug.Assert(_assassinationTarget != null);

        EAssassinationType assassinationType;
        float yPositionDiff = Mathf.Abs(transform.position.y - _assassinationTarget.position.y);
        if (transform.position.y > _assassinationTarget.position.y &&
            yPositionDiff >= _assassinationData.fallAssassinationHeightThreshold)
        {
            assassinationType = EAssassinationType.Fall;
        }
        else if (transform.position.y < _assassinationTarget.position.y &&
                 yPositionDiff >= _assassinationData.jumpAssassinationHeightThreshold)
        {
            assassinationType = EAssassinationType.Jump;
        }
        else
        {
            assassinationType = EAssassinationType.Ground;
        }

        StartCoroutine(AssassinateRoutine(assassinationType));
    }

    private IEnumerator AssassinateRoutine(EAssassinationType assassinationType)
    {
        _isAssassinating = true;

        float assassinationDuration = 0f;
        switch (assassinationType)
        {
            case EAssassinationType.Ground:
                assassinationDuration = _assassinationData.groundAssassinationDuration;
                break;
            case EAssassinationType.Jump:
                assassinationDuration = _assassinationData.jumpAssassinationDuration;
                break;
            case EAssassinationType.Fall:
                assassinationDuration = _assassinationData.fallAssassinationDuration;

                while (YVelocity > 0f)
                {
                    yield return null;
                }

                break;
            default:
                Debug.Assert(false);
                break;
        }

        float t = 0f;
        Vector3 initialPos = transform.position;

        while (t <= assassinationDuration)
        {
            float alpha = t / assassinationDuration;
            transform.position = Vector3.Lerp(initialPos, _assassinationTarget.position, alpha * alpha * alpha);

            yield return null;
            t += Time.deltaTime;
        }

        // 임시 암살 처리
        Destroy(_assassinationTarget.gameObject);

        // TODO: 실제 적에게 데미지를 입혀야함

        _isAssassinating = false;
    }

    private void HandleRunAction()
    {
        if (CameraController.Instance.IsOnChangeHeightRoutine)
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
        if (!IsGrounded || CheckPlayerState(EPlayerState.Run) || CameraController.Instance.IsOnChangeHeightRoutine)
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
