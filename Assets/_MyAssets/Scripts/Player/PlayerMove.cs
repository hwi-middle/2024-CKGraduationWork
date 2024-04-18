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
    Hide = 1 << 7,
    Peek = 1 << 8,
    Alive = 1 << 9,
    Dead = 1 << 10,
    WireAction = 1 << 11,
}

public class PlayerMove : Singleton<PlayerMove>
{
    private Camera _camera;
    
    [SerializeField] private PlayerInputData _inputData;
    private int _currentState = (int)EPlayerState.Idle | (int)EPlayerState.Alive;
    
    [Header("Player Data")]
    [SerializeField] private PlayerData _playerData;

    private GameObject _playerCanvas;
    private GameObject _wireAvailableUI;

    [Header("WirePoint Offset")] [SerializeField]
    private float _wirePointOffset;

    private TMP_Text _stateText;

    private RectTransform _wireAvailableUiRectTransform;
    private IEnumerator _wireActionRoutine;

    private Vector3 _targetPosition;
    private Vector3 _wireHangPosition;

    private float _playerApplySpeed;

    private static readonly Vector3 CAMERA_CENTER_POINT = new(0.5f, 0.5f, 0.0f);

    private readonly int _stateCount = Enum.GetValues(typeof(EPlayerState)).Length;

    private bool IsOnWire => _wireActionRoutine != null;
    
    // Player Respawn
    private IEnumerator _movePlayerToRespawnPointRoutine;
    [SerializeField] private float _respawnMoveDuration;

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
        _inputData.runEvent += HandleRunAction;
        _inputData.runQuitEvent += HandleQuitRunAction;
        _inputData.wireEvent += HandleWireAction;
        _inputData.crouchEvent += HandleCrouchAction;
    }

    private void OnDisable()
    {
        _inputData.moveEvent -= HandleMoveAction;
        _inputData.runEvent -= HandleRunAction;
        _inputData.runQuitEvent -= HandleQuitRunAction;
        _inputData.wireEvent -= HandleWireAction;
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
            ShowWirePointUI();
            RotatePlayer();
            MovePlayer();
        }

        UpdatePlayerStateText();

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

    // Reset Player Position To CheckPoint
    public void ResetPlayerPosition()
    {
        if (_movePlayerToRespawnPointRoutine != null)
        {
            return;
        }

        _movePlayerToRespawnPointRoutine = MovePlayerToRespawnPointRoutine();
        StartCoroutine(_movePlayerToRespawnPointRoutine);
    }

    private IEnumerator MovePlayerToRespawnPointRoutine()
    {
        Vector3 startPosition = transform.position;
        Vector3 checkPoint = RespawnHelper.Instance.LastCheckPoint;

        float t = 0;
        while (t <= _respawnMoveDuration)
        {
            float alpha = t / _respawnMoveDuration;
            transform.position = Vector3.Lerp(startPosition, checkPoint, alpha * alpha * alpha);
            yield return null;
            t += Time.deltaTime;
        }

        Vector3 lookDirection = (RespawnHelper.Instance.LastDeadPoint - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(lookDirection);
        transform.position = checkPoint;
        SetInitState();
        CameraController.Instance.AlignCameraToPlayer();
        RespawnHelper.Instance.PlayerModel.SetActive(true);
        MiddleSaveData.Instance.LoadSavedData();
        _movePlayerToRespawnPointRoutine = null;
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
        // NonAlloc -> 0 일때는 충돌 없음, 1 일때는 둘중 하나의 충돌이 있었음
        // 후에 NonAlloc으로 변경
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
        if (CameraController.Instance.IsOnRoutine)
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
        if (!IsGrounded || CheckPlayerState(EPlayerState.Run) || CameraController.Instance.IsOnRoutine)
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
