using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // Wire Variable
    private enum ECalculateType
    {
        V2,
        V3
    }

    [Header("Player Data")] [SerializeField]
    private PlayerWireData _myData;

    [Header("WirePoint Variable")] [SerializeField]
    private GameObject _wireAvailableUI;

    [SerializeField] private RectTransform _wireAvilableUITransform;
    private IEnumerator _wireAction;

    private Vector3 _targetPosition;
    private Vector3 _wireHangPosition;

    private static readonly Vector3 CAMERA_CENTER_POINT = new(0.5f, 0.5f, 0.0f);

    private bool _isOnWire;

    private List<GameObject> WirePoints
    {
        get
        {
            List<GameObject> detectWirePoint = new();
            Vector3 playerPos = transform.position;
            Collider[] objectsInRange = Physics.OverlapSphere(playerPos, _myData.maxWireDistance);

            foreach (var obj in objectsInRange)
            {
                if (!obj.CompareTag("WirePoint") ||
                    CalculateDistance(playerPos, obj.transform.position, ECalculateType.V3) < _myData.minWireDistance)
                {
                    continue;
                }

                Vector3 wirePointPos = obj.transform.position;
                Vector3 viewportPos = _camera.WorldToViewportPoint(wirePointPos);

                if (viewportPos.z < 0)
                {
                    continue;
                }

                detectWirePoint.Add(obj.gameObject);
            }

            return detectWirePoint;
        }
    }

    private bool CanHangWire
    {
        get
        {
            Debug.Assert(_camera != null, "_mainCamera != null");

            Ray ray = _camera.ViewportPointToRay(CAMERA_CENTER_POINT);
            bool isHit = Physics.Raycast(ray, out RaycastHit hit);
            if (!isHit || !hit.transform.CompareTag("WirePoint"))
            {
                return false;
            }

            float distance = CalculateDistance(hit.transform.position, transform.position, ECalculateType.V3);

            if (distance > _myData.maxWireDistance || distance < _myData.minWireDistance)
            {
                return false;
            }

            _targetPosition = hit.transform.position;
            _wireHangPosition = hit.point;

            const float TARGET_POSITION_Y_ADDITIVE = 2.0f;
            _targetPosition.y += TARGET_POSITION_Y_ADDITIVE;

            return true;
        }
    }

    private bool IsCollideWhenWireAction
    {
        get
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _controller.radius);

            foreach (var coll in colliders)
            {
                if (coll.transform.CompareTag("Ground") || coll.CompareTag("Wall"))
                {
                    return true;
                }
            }

            return false;
        }
    }

    // Assassinate Variable
    private enum EAssassinationType
    {
        Ground, // 평지에서 암살
        Fall, // 위에서 아래로 떨어지며 암살
        Jump, // 아래에서 위로 점프하며 암살
    }

    [SerializeField] private PlayerAssassinationData _assassinationData;

    // [SerializeField] private TMP_Text _noteTextForDebug;
    private bool _isAssassinating = false;
    private Transform _assassinationTarget;

    // Player Move Variable
    [SerializeField] private float _jumpHeight;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _slideSpeed;

    [Header("Gravity Scale")] [SerializeField]
    private float _gravityMultiplier;

    private float _yVelocity;
    protected float YVelocity => _yVelocity;

    private Camera _camera;

    private Vector3 _inputDirection;
    private Vector3 _velocity;

    private CharacterController _controller;

    private Vector3 _hitNormal;
    private bool _isSliding;
    private Vector3 _slideVelocity;

    private bool IsGrounded => _controller.isGrounded;

    protected virtual void Awake()
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
        ShowWirePointUI();
        RotatePlayer();
        MovePlayer();

        if (!_isAssassinating)
        {
            _assassinationTarget = GetAimingEnemy();
        }
    }

    private void RotatePlayer()
    {
        Debug.Assert(_camera != null, "_camera != null");

        if (_inputDirection.sqrMagnitude == 0 || _isOnWire)
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

        if (_isOnWire && _yVelocity < 0)
        {
            _yVelocity = 0;
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

    // Wire Methods
    private float CalculateDistance(Vector3 origin, Vector3 target, ECalculateType type)
    {
        return type == ECalculateType.V3 ? (origin - target).magnitude : ((Vector2)origin - (Vector2)target).magnitude;
    }

    private GameObject FindWirePoint()
    {
        List<GameObject> wirePointInScreen = WirePoints;

        if (wirePointInScreen.Count == 0)
        {
            return null;
        }

        float minDistanceFromCenter = 2.0f;
        GameObject nearWirePoint = null;

        foreach (var wirePoint in wirePointInScreen)
        {
            Vector3 wirePointPos = wirePoint.transform.position;
            Vector3 viewportPos = _camera.WorldToViewportPoint(wirePointPos);

            if (viewportPos.z < 0)
            {
                continue;
            }

            float distanceFromCenter = CalculateDistance(CAMERA_CENTER_POINT, viewportPos, ECalculateType.V2);
            const float RESTRICT_RANGE_FROM_CENTER = 0.15f;

            if (distanceFromCenter > minDistanceFromCenter || distanceFromCenter > RESTRICT_RANGE_FROM_CENTER)
            {
                continue;
            }

            minDistanceFromCenter = distanceFromCenter;
            nearWirePoint = wirePoint;
        }

        // nearWirePoint 가 있다면 플레이어 사이의 장매물이 있는지 확인 후 Return
        return nearWirePoint == null ? null : CheckObstacle(nearWirePoint, transform.position);
    }

    private GameObject CheckObstacle(GameObject nearWirePoint, Vector3 playerPos)
    {
        const float RAY_POSITION_Y_TOLERANCE = 0.5f;
        playerPos.y += RAY_POSITION_Y_TOLERANCE;

        Vector3 nearWirePointPos = nearWirePoint.transform.position;
        Vector3 rayDirection = (nearWirePointPos - playerPos).normalized;
        float rayDistance = CalculateDistance(playerPos, nearWirePointPos, ECalculateType.V3);

        Ray ray = new Ray(playerPos, rayDirection * rayDistance);
        Physics.Raycast(ray, out RaycastHit hit);

        return !hit.transform.CompareTag("WirePoint") ? null : nearWirePoint;
    }

    private void ShowWirePointUI()
    {
        if (_wireAction != null)
        {
            _wireAvailableUI.SetActive(false);
            return;
        }

        Debug.Assert(_wireAvilableUITransform != null);

        GameObject wirePoint = FindWirePoint();

        if (wirePoint == null)
        {
            _wireAvailableUI.SetActive(false);
            return;
        }


        _wireAvailableUI.SetActive(true);

        Vector3 wireScreenPoint = _camera.WorldToScreenPoint(wirePoint.transform.position);

        const float OFFSET = 50.0f;

        wireScreenPoint.x += OFFSET;

        _wireAvilableUITransform.position = wireScreenPoint;
    }

    public void OnWireButtonClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }

        if (!CanHangWire)
        {
            return;
        }

        ApplyWireAction();
    }

    private void ApplyWireAction()
    {
        if (_wireAction != null)
        {
            return;
        }

        if (!_wireAvailableUI.activeSelf)
        {
            return;
        }

        PerformJump();
        
        Quaternion cameraRotation = _camera.transform.localRotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);
        
        _wireAction = WireActionRoutine();
        StartCoroutine(_wireAction);
    }

    private IEnumerator WireActionRoutine()
    {
        while (YVelocity > 0)
        {
            yield return null;
        }

        _isOnWire = true;
        Vector3 initPos = transform.position;
        float t = 0;

        LineDraw.Instance.TurnOnLine();

        while (t <= _myData.wireActionDuration && !IsCollideWhenWireAction)
        {
            float alpha = t / _myData.wireActionDuration;

            transform.position = Vector3.Lerp(initPos, _targetPosition, alpha * alpha * alpha);

            LineDraw.Instance.Draw(transform.position, _wireHangPosition);

            yield return null;
            t += Time.deltaTime;
        }

        LineDraw.Instance.TurnOffLine();
        _wireAction = null;
        _isOnWire = false;
    }

    // Assassinate Methods
    public void OnAssassinateKeyDown(InputAction.CallbackContext context)
    {
        if (!context.started || _assassinationTarget == null)
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
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * assassinateDistance, Color.green,
            0f, false);
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("BypassAiming"));

        if (Physics.Raycast(ray, out RaycastHit hit, assassinateDistance, layerMask) &&
            hit.transform.CompareTag("AssassinationTarget"))
        {
            // _noteTextForDebug.text = $"Current Target: {hit.transform.name}";
            return hit.transform;
        }

        // _noteTextForDebug.text = "Current Target: None";
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

        // TODO: 실제 적에게 데미지를 입혀야함

        _isAssassinating = false;
    }
}