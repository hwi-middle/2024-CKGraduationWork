using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[Flags]
public enum EPlayerState
{
    None = 0,
    Idle = 1 << 1,
    Walk = 1 << 2,
    Run = 1 << 3,
    Crouch = 1 << 4,
    Jump = 1 << 5,
    Hide = 1 << 6,
    Peek = 1 << 7,
    Alive = 1 << 8,
    Dead = 1 << 9,
    WireAction = 1 << 10,
    Sliding = 1 << 11
}

public class PlayerMove : Singleton<PlayerMove>
{
    private Camera _camera;
    
    [SerializeField] private PlayerInputData _inputData;
    private int _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;
    
    [Header("Player Base Data")]
    [SerializeField] private PlayerData _playerData;

    private GameObject _playerCanvas;
    private GameObject _wireAvailableUI;

    [Header("WirePoint Offset")] [SerializeField]
    private float _wirePointOffset;

    private TMP_Text _stateText;

    // private int _hp;

    private RectTransform _wireAvailableUiRectTransform;
    private IEnumerator _wireActionRoutine;

    private Vector3 _targetPosition;
    private Vector3 _wireHangPosition;

    private float _playerApplySpeed;

    private static readonly Vector3 CAMERA_CENTER_POINT = new(0.5f, 0.5f, 0.0f);

    private readonly int _stateCount = Enum.GetValues(typeof(EPlayerState)).Length;

    private bool IsOnWire => _wireActionRoutine != null;

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
    protected float YVelocity => _yVelocity;

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
        _wireAvailableUI = _playerCanvas.transform.Find("WireAvailable").gameObject;
        _stateText = _playerCanvas.transform.Find("PlayerStateText").transform.GetComponent<TMP_Text>();
        _wireAvailableUiRectTransform = _wireAvailableUI.GetComponent<RectTransform>();

        Instantiate(_playerData.lineRendererPrefab);
        LineDrawHelper.Instance.DisableLine();
    }

    private void OnEnable()
    {
        _inputData.moveEvent += HandleMoveAction;
        _inputData.jumpEvent += HandleJumpAction;
        _inputData.runEvent += HandleRunAction;
        _inputData.runQuitEvent += HandleQuitRunAction;
        _inputData.assassinateEvent += HandleAssassinateAction;
        _inputData.wireEvent += HandleWireAction;
        _inputData.crouchEvent += HandleCrouchAction;
    }

    private void OnDisable()
    {
        _inputData.moveEvent -= HandleMoveAction;
        _inputData.jumpEvent -= HandleJumpAction;
        _inputData.runEvent -= HandleRunAction;
        _inputData.runQuitEvent -= HandleQuitRunAction;
        _inputData.assassinateEvent -= HandleAssassinateAction;
        _inputData.wireEvent -= HandleWireAction;
        _inputData.crouchEvent -= HandleCrouchAction;
    }

    private void Start()
    {
        Debug.Assert(_controller != null, "_controller !=null");

        // Cursor Visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // _hp = _playerData.playerHp;
        _camera = Camera.main;
    }


    protected virtual void Update()
    {
        // 사망 상태 테스트 용 임시 입력
        // if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     _hp = 0;
        // }

        // CheckAndSwitchLifeState();
        SetSlideVelocity();
        ShowWirePointUI();
        RotatePlayer();
        MovePlayer();


        if (IsGrounded)
        {
            RemovePlayerState(EPlayerState.Jump);
        }

        UpdatePlayerStateText();

        if (!_isAssassinating)
        {
            _assassinationTarget = GetAimingEnemy();
        }
    }

    // private void CheckAndSwitchLifeState()
    // {
    //     if (_hp > 0)
    //     {
    //         return;
    //     }
    //     
    //     RemovePlayerState(EPlayerState.Alive);
    //     AddPlayerState(EPlayerState.Dead);
    //
    //     Destroy(gameObject);
    // }

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

        if (_inputDirection.sqrMagnitude == 0 || IsOnWire)
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
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);
    }

    private void SetSlideVelocity()
    {
        if (IsOnSlope())
        {
            _slideVelocity = Vector3.ProjectOnPlane(new Vector3(0, _velocity.y, 0), _hitNormal);
            _isSliding = true;
            return;
        }

        _slideVelocity = Vector3.zero;
        _isSliding = false;
    }

    private bool IsOnSlope()
    {
        if (_isSliding)
        {
            return !IsBetweenSlopeAndGround();
        }

        if (_hitObject == null || !IsGrounded || SlideableZone.slideableZoneCount == 0)
        {
            return false;
        }

        float angle = Vector3.Angle(Vector3.up, _hitNormal);
        bool isOnSlope = Mathf.FloorToInt(angle) >= _controller.slopeLimit && Mathf.CeilToInt(angle) < 90.0f;

        return isOnSlope && !IsBetweenSlopeAndGround();
    }

    private bool IsBetweenSlopeAndGround()
    {
        // Ground에 Ray가 닿았을 때 Ground와의 거리가 최소 슬라이드 높이 이하면 _isSliding -> false
        Vector3 bottom = transform.position - new Vector3(0, _controller.height / 2, 0);
        Ray ray = new Ray(bottom, Vector3.down);
        const float RAY_DISTANCE = 3.0f;
        if (Physics.Raycast(ray, out RaycastHit hit, RAY_DISTANCE))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (Mathf.CeilToInt(angle) <= _controller.slopeLimit)
            {
                float heightFromHit = bottom.y - hit.point.y;

                // 최소 슬라이딩 높이
                const float MIN_SLIDE_HEIGHT = 0.1f;
                if (heightFromHit < MIN_SLIDE_HEIGHT)
                {
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _hitObject = hit.gameObject;
        _hitNormal = hit.normal;
    }

    private void MovePlayer()
    {
        if (_isSliding)
        {
            _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive | (int)EPlayerState.Sliding;
            _velocity = _slideVelocity;

            ApplyGravity();

            _controller.Move(_playerData.slopeSlideSpeed * Time.deltaTime * _velocity);
            return;
        }

        if (CheckPlayerState(EPlayerState.Sliding))
        {
            RemovePlayerState(EPlayerState.Sliding);
        }

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

        if (IsOnWire && _yVelocity < 0)
        {
            _yVelocity = 0;
        }

        _velocity.y = _yVelocity;
    }

    private void UpdatePlayerStateText()
    {
        _stateText.text = "Cur State : ";
        EPlayerState state = EPlayerState.None;

        for (int i = 1; i <= _stateCount; i++)
        {
            if (((1 << i) & _currentState) != 0)
            {
                _stateText.text += $"{state + (1 << i)} ";
            }
        }
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
        if (IsOnWire)
        {
            return;
        }

        _inputDirection = new Vector3(pos.x, 0, pos.y);

        if (_inputDirection.sqrMagnitude == 0)
        {
            RemovePlayerState(EPlayerState.Walk);
            return;
        }

        AddPlayerState(EPlayerState.Walk);
    }

    private void HandleJumpAction()
    {
        if (!IsGrounded || _isSliding)
        {
            return;
        }

        if (CheckPlayerState(EPlayerState.Crouch))
        {
            RemovePlayerState(EPlayerState.Crouch);
            return;
        }

        PerformJump();
    }

    protected void PerformJump()
    {
        AddPlayerState(EPlayerState.Jump);
        _yVelocity += _playerData.jumpHeight;
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

    private bool CanHangWireToWirePoint()
    {
        if (IsOnWire)
        {
            return false;
        }

        Debug.Assert(_camera != null, "_camera != null");

        Ray ray = _camera.ViewportPointToRay(CAMERA_CENTER_POINT);
        float distanceOfCameraFromPlayer = (_camera.transform.position - transform.position).magnitude;
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, _playerData.maxWireDistance + distanceOfCameraFromPlayer,
            LayerMask.GetMask("WirePoint"));

        if (!isHit)
        {
            return false;
        }

        float distance = (hit.transform.position - transform.position).magnitude;


        if (distance < _playerData.minWireDistance)
        {
            return false;
        }

        _targetPosition = hit.transform.position;
        _wireHangPosition = hit.point;

        _targetPosition.y += _wirePointOffset;

        return true;
    }

    private bool IsCollideWhenWireAction()
    {
        int detectLayer = LayerMask.GetMask("Ground") + LayerMask.GetMask("Wall");

        // 매 프레임 호출 시 문제 발생 가능성 높음 -> 교체 방안 연구
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, _controller.radius, detectLayer);

        return overlappedColliders.Length != 0;
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

    private void ShowWirePointUI()
    {
        if (_wireActionRoutine != null)
        {
            _wireAvailableUI.SetActive(false);
            return;
        }

        Debug.Assert(_wireAvailableUI != null);

        GameObject wirePoint = FindNearestWirePointFromAim();

        if (wirePoint == null)
        {
            _wireAvailableUI.SetActive(false);
            return;
        }

        Vector3 wirePointPosition = wirePoint.transform.position;
        Vector3 playerPosition = transform.position;

        Vector3 rayDirection = (wirePointPosition - playerPosition).normalized;
        float rayDistance = (wirePointPosition - playerPosition).magnitude;

        Ray ray = new Ray(playerPosition, rayDirection);

        Physics.Raycast(ray, out RaycastHit hit, rayDistance);

        Debug.Assert(hit.transform != null, "hit != null");

        if (!hit.transform.CompareTag("WirePoint"))
        {
            _wireAvailableUI.SetActive(false);
            return;
        }

        _wireAvailableUI.SetActive(true);

        Vector3 wireScreenPoint = _camera.WorldToScreenPoint(wirePointPosition);

        const float OFFSET = 50.0f;

        wireScreenPoint.x += OFFSET;

        _wireAvailableUiRectTransform.position = wireScreenPoint;
    }

    private void HandleWireAction()
    {
        if (!CanHangWireToWirePoint())
        {
            return;
        }

        ApplyWireAction();
    }

    private void ApplyWireAction()
    {
        if (_wireActionRoutine != null)
        {
            return;
        }

        if (!_wireAvailableUI.activeSelf)
        {
            return;
        }

        _inputDirection = Vector3.zero;

        if (IsGrounded)
        {
            PerformJump();
        }

        _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;

        Quaternion cameraRotation = _camera.transform.localRotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);

        _wireActionRoutine = WireActionRoutine();
        StartCoroutine(_wireActionRoutine);
    }

    private IEnumerator WireActionRoutine()
    {
        AddPlayerState(EPlayerState.WireAction);
        while (YVelocity > 0)
        {
            yield return null;
        }

        Vector3 initPos = transform.position;
        float t = 0;

        LineDrawHelper.Instance.EnableLine();

        while (t <= _playerData.wireActionDuration && !IsCollideWhenWireAction())
        {
            float alpha = t / _playerData.wireActionDuration;

            transform.position = Vector3.Lerp(initPos, _targetPosition, alpha * alpha * alpha);

            LineDrawHelper.Instance.DrawWire(transform.position, _wireHangPosition);

            yield return null;
            t += Time.deltaTime;
        }

        LineDrawHelper.Instance.DisableLine();
        RemovePlayerState(EPlayerState.WireAction);
        _wireActionRoutine = null;
    }

    private void HandleAssassinateAction()
    {
        if (_assassinationTarget == null)
        {
            return;
        }

        Assassinate();
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
                PerformJump();

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
        if (CheckPlayerState(EPlayerState.Jump))
        {
            RemovePlayerState(EPlayerState.Run);
            return;
        }

        // Run과 Crouch 상태 중 우선 순위는 Run
        RemovePlayerState(EPlayerState.Crouch);
        AddPlayerState(EPlayerState.Run);
    }

    private void HandleQuitRunAction()
    {
        RemovePlayerState(EPlayerState.Run);
    }

    private void HandleCrouchAction()
    {
        if (!IsGrounded || CheckPlayerState(EPlayerState.Run))
        {
            return;
        }

        if (CheckPlayerState(EPlayerState.Crouch))
        {
            RemovePlayerState(EPlayerState.Crouch);
            return;
        }

        AddPlayerState(EPlayerState.Crouch);
    }
}
