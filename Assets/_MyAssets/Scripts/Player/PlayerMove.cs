using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerWireData _myData;

    [Header("WirePoint Variable")]
    [SerializeField] private GameObject _wireAvailableUI;

    private RectTransform _wireAvailableUiRectTransform;
    private IEnumerator _wireActionRoutine;

    private Vector3 _targetPosition;
    private Vector3 _wireHangPosition;

    private static readonly Vector3 CAMERA_CENTER_POINT = new(0.5f, 0.5f, 0.0f);

    private bool IsOnWire => _wireActionRoutine != null;

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

    [Header("Gravity Scale")]
    [SerializeField] private float _gravityMultiplier;

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
        _wireAvailableUiRectTransform = _wireAvailableUI.GetComponent<RectTransform>();
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

        if (_inputDirection.sqrMagnitude == 0 || IsOnWire)
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

        if (IsOnWire && _yVelocity < 0)
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

    private List<GameObject> GetWirePoints()
    {
        List<GameObject> detectedWirePoints = new();
        Vector3 playerPos = transform.position;

        Collider[] wirePointsInRange =
            Physics.OverlapSphere(playerPos, _myData.maxWireDistance, LayerMask.GetMask("WirePoint"));

        foreach (Collider wirePoint in wirePointsInRange)
        {
            Vector3 wirePointPos = wirePoint.transform.position;
            float distance = (wirePointPos - playerPos).magnitude;
            if (distance < _myData.minWireDistance || _camera.WorldToViewportPoint(wirePointPos).z < 0)
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

        bool isHit = Physics.Raycast(ray, out RaycastHit hit, _myData.maxWireDistance, LayerMask.GetMask("WirePoint"));

        if (!isHit)
        {
            return false;
        }

        float distance = (hit.transform.position - transform.position).magnitude;

        if (distance < _myData.minWireDistance || distance > _myData.maxWireDistance)
        {
            return false;
        }

        _targetPosition = hit.transform.position;
        _wireHangPosition = hit.point;

        const float ADDITIVE_VALUE = 2.0f;
        _targetPosition.y += ADDITIVE_VALUE;

        return true;
    }

    private bool IsCollideWhenWireAction()
    {
        int detectLayer = LayerMask.GetMask("Ground") + LayerMask.GetMask("Wall");
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
        GameObject nearWirePoint = null;

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
            nearWirePoint = wirePoint;
        }

        return nearWirePoint;
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

        _wireAvailableUI.SetActive(true);

        Vector3 wireScreenPoint = _camera.WorldToScreenPoint(wirePoint.transform.position);

        const float OFFSET = 50.0f;

        wireScreenPoint.x += OFFSET;

        _wireAvailableUiRectTransform.position = wireScreenPoint;
    }

    public void OnWireButtonClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }

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

        PerformJump();

        Quaternion cameraRotation = _camera.transform.localRotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f);

        _wireActionRoutine = WireActionRoutine();
        StartCoroutine(_wireActionRoutine);
    }

    private IEnumerator WireActionRoutine()
    {
        while (YVelocity > 0)
        {
            yield return null;
        }

        Vector3 initPos = transform.position;
        float t = 0;

        WireLineDrawHelper.Instance.ChangeLineEnable(true);

        while (t <= _myData.wireActionDuration && !IsCollideWhenWireAction())
        {
            float alpha = t / _myData.wireActionDuration;

            transform.position = Vector3.Lerp(initPos, _targetPosition, alpha * alpha * alpha);

            WireLineDrawHelper.Instance.Draw(transform.position, _wireHangPosition);

            yield return null;
            t += Time.deltaTime;
        }

        WireLineDrawHelper.Instance.ChangeLineEnable(false);
        _wireActionRoutine = null;
    }

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