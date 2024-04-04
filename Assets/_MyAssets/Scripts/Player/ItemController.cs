using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private GameObject _pointObject;

    private GameObject _itemPrefab;

    private GameObject _itemInHand;
    private Camera _mainCamera;

    private bool _isOnAiming;
    private bool _isOverItemRange;
    private bool _isNotHit;

    private Transform _shootPoint;
    private Vector3 _throwTargetPoint;
    private Vector3 _shootDirection;
    private Vector3 _prevForward;
    private Vector3 _prevThrowTargetPoint;

    private void Awake()
    {
        _shootPoint = transform.Find("ShootPoint").GetComponent<Transform>();
        _itemPrefab = Resources.Load<GameObject>("Items/SoundBomb");
    }

    private void OnEnable()
    {
        _inputData.aimingEvent += HandleAiming;
        _inputData.aimingCancelEvent += HandleAimingCancel;
        _inputData.shootEvent += HandleShoot;
    }

    private void OnDisable()
    {
        _inputData.aimingEvent -= HandleAiming;
        _inputData.aimingCancelEvent -= HandleAimingCancel;
        _inputData.shootEvent -= HandleShoot;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _pointObject.transform.localScale =
            new Vector3(_playerData.itemImpactRadius, 0.1f, _playerData.itemImpactRadius);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _itemInHand = _itemPrefab;
        }
        
        if (_isOnAiming)
        {
            SetThrowTargetPosition();
            
            if (_isNotHit)
            {
                LineDrawHelper.Instance.DisableLine();
                return;
            }
            
            LineDrawHelper.Instance.EnableLine();
            PlayerMove.Instance.AlignPlayerToCameraForward();
        }
    }

    private void HandleAiming()
    {
        _isOnAiming = true;
        PlayerMove.Instance.ChangeCameraToAiming();
        _pointObject.SetActive(true);
    }

    private void HandleAimingCancel()
    {
        _isOnAiming = false;
        PlayerMove.Instance.ChangeCameraToFreeLook();
        LineDrawHelper.Instance.DisableLine();
        _pointObject.SetActive(false);
    }

    private void SetThrowTargetPosition()
    {
        Transform cameraTransform = _mainCamera.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 playerPosition = transform.position;
        Vector3 cameraForward = cameraTransform.forward;
        _pointObject.transform.position = Vector3.zero - Vector3.down * 10000;

        Ray ray = new Ray(cameraPosition, cameraForward);
        LayerMask layerMask = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Item"));
        _isNotHit = false;

        if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
        {
            _isNotHit = true;
            return;
        }
        
        Vector3 cameraPositionOnPlane = Vector3.ProjectOnPlane(cameraPosition, Vector3.up);
        Vector3 playerPositionOnPlane = Vector3.ProjectOnPlane(playerPosition, Vector3.up);
        Vector3 hitPositionOnPlane = Vector3.ProjectOnPlane(hit.point, Vector3.up);
        
        float playerDistanceFromCamera = (cameraPositionOnPlane - playerPositionOnPlane).magnitude;
        float hitPointDistanceFromCamera = (hitPositionOnPlane - cameraPositionOnPlane).magnitude;
        float hitPointDistanceFromPlayer = hitPointDistanceFromCamera - playerDistanceFromCamera;
        
        _isOverItemRange = hitPointDistanceFromPlayer > _playerData.maxItemRange;

        if (!_isOverItemRange)
        {
            _throwTargetPoint = cameraForward * hitPointDistanceFromPlayer + playerPosition;
            _throwTargetPoint.y = hit.point.y;

            _throwTargetPoint = Vector3.Lerp(_prevThrowTargetPoint, _throwTargetPoint, 0.2f);

            _prevThrowTargetPoint = _throwTargetPoint;
            _prevForward = cameraForward;

            DrawParabola();

            _pointObject.transform.position = Vector3.ProjectOnPlane(_throwTargetPoint, hit.normal).normalized * 2.0f;
            _pointObject.transform.localRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            return;
        }

        Vector3 playerForward = transform.forward;

        _throwTargetPoint = playerForward * _playerData.maxItemRange + playerPosition;
        Debug.DrawRay(playerPosition, _throwTargetPoint.normalized, Color.red);
        Debug.DrawRay(_throwTargetPoint, Vector3.up, Color.green);
        Ray downRay = new Ray(_throwTargetPoint, Vector3.down);
        if (Physics.Raycast(downRay, out hit, float.MaxValue))
        {
            _throwTargetPoint.y = hit.point.y;
        }

        // Ray가 최대 거리를 벗어났을 때 마우스의 이동이 있을 경우를 대비한 Target 조정
        cameraForward = Vector3.ProjectOnPlane(cameraForward, Vector3.up);
        _prevForward = Vector3.ProjectOnPlane(_prevForward, Vector3.up);
        cameraForward.Normalize();
        _prevForward.Normalize();

        _prevForward = Vector3.Slerp(_prevForward, cameraForward, 0.2f);

        _throwTargetPoint = playerPosition + _prevForward.normalized * _playerData.maxItemRange;
        _throwTargetPoint.y = _prevThrowTargetPoint.y;

        DrawParabola();
        _pointObject.transform.position = Vector3.ProjectOnPlane(_throwTargetPoint, hit.normal).normalized;
    }

    private void DrawParabola(bool greaterAngle = false)
    {
        float angleInRadian = GetParabolaShootingAngleInRadian(greaterAngle);
        Vector3 originPosition = _shootPoint.position;
        originPosition.y += 0.5f;
        
        Vector3 targetDirection = _throwTargetPoint - originPosition;
        float targetDistance = targetDirection.magnitude;
        targetDirection.Normalize();
        
        if (float.IsNaN(angleInRadian))
        {
            return;
        }

        _shootPoint.localRotation = Quaternion.Euler(-angleInRadian * Mathf.Rad2Deg, 0f, 0f);
        float gravity = Mathf.Abs(Physics.gravity.y);
        float v0x = _playerData.throwPower * Mathf.Cos(angleInRadian);
        float v0y = _playerData.throwPower * Mathf.Sin(angleInRadian);
        float a = -gravity / (2 * v0x * v0x);
        float tanAngle = v0y / v0x;
        
        const float DIVIDE = 0.2f;
        int count = Mathf.CeilToInt(targetDistance / DIVIDE);
        LineDrawHelper.Instance.SetPositionCount(count);
        Vector3[] list = new Vector3[count];
        float xPrime = 0f;
        for (int i = 0; i < count; i++)
        {
            Vector3 relativePosition = targetDirection * xPrime;
            relativePosition.y = a * xPrime * xPrime + tanAngle * xPrime;
            list[i] = originPosition + relativePosition;
            xPrime += DIVIDE;
        }
        
        LineDrawHelper.Instance.DrawParabola(list);
    }

    private float GetParabolaShootingAngleInRadian(bool greaterAngle)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 originPosition = _shootPoint.position; 
        originPosition.y += 0.5f;

        Vector3 relativeTarget = _throwTargetPoint - originPosition;
        float x1Square = relativeTarget.x * relativeTarget.x + relativeTarget.z * relativeTarget.z;
        float x1 = Mathf.Sqrt(x1Square);
        float y1 = relativeTarget.y;
        
        float k = -(gravity * x1Square) / (2 * _playerData.throwPower * _playerData.throwPower);
        float determiner = x1Square - 4f * k * (k - y1);

        if (determiner < 0)
        {
            return float.NaN;
        }

        float sign = greaterAngle ? -1f : 1f;
        float angle = Mathf.Atan((-x1 + Mathf.Sqrt(determiner) * sign) / (2f * k));
        return angle;
    }

    private void HandleShoot()
    {
        if (_itemInHand == null)
        {
            return;
        }

        Debug.Assert(_mainCamera != null, "_mainCamera != null");
        Transform cameraTransform = _mainCamera.transform;
        Vector3 playerPosTop = transform.position;
        playerPosTop.y += 1.0f;
        GameObject itemPrefab =
            Instantiate(_itemInHand, playerPosTop, cameraTransform.localRotation);
        Rigidbody itemRigidbody = itemPrefab.GetComponent<Rigidbody>();
        itemPrefab.GetComponent<ItemHandler>().Init(_playerData.itemGaugeAmount, _playerData.itemImpactRadius);
        itemRigidbody.AddForce(_shootPoint.forward * _playerData.throwPower, ForceMode.VelocityChange);
        
        _itemInHand = null;
        HandleAimingCancel();
    }
}