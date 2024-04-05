using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerInputData _inputData;
    
    private GameObject _itemPrefab;
    private GameObject _itemShowPrefabInstance;

    private GameObject _itemInHand;
    private Camera _mainCamera;

    private bool _isOnAiming;
    private bool _isOverItemRange;
    private bool _isNotHit;

    private Transform _shootPoint;
    private Vector3 _throwTargetPoint;
    private Vector3 _shootDirection;
    private Vector3 _prevPosition;

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
                RemoveTargetPoint();
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
    }

    private void HandleAimingCancel()
    {
        _isOnAiming = false;
        PlayerMove.Instance.ChangeCameraToFreeLook();
        LineDrawHelper.Instance.DisableLine();
        RemoveTargetPoint();
    }

    private void SetThrowTargetPosition()
    {
        Transform cameraTransform = _mainCamera.transform;
        Vector3 cameraForward = cameraTransform.forward;
        Transform playerTransform = transform;
        Vector3 playerPosition = playerTransform.position;
        Vector3 playerForward = playerTransform.forward;

        Ray ray = new Ray(playerPosition, cameraForward);
        
        LayerMask layerMask = ~(LayerMask.GetMask("Player") 
                                | LayerMask.GetMask("Item")
                                | LayerMask.GetMask("Enemy"));
        _isNotHit = false;
        
        // Ray가 안닿을 일을 없다고 가정하였지만 예외적인 케이스를 대비한 isNotHit
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            _isNotHit = true;
            return;
        }

        // point와의 거리를 구하기 위해 수평선상에 정렬
        Vector3 playerPositionOnPlane = Vector3.ProjectOnPlane(playerPosition, Vector3.up);
        Vector3 hitPositionOnPlane = Vector3.ProjectOnPlane(hit.point, Vector3.up);

        // point와 플레이어의 거리
        float hitPointDistanceFromPlayer = (playerPositionOnPlane - hitPositionOnPlane).magnitude;
        
        // point와 플레이어의 거리가 아이템 최대 거리보다 크면 overItemRange : true
        _isOverItemRange = hitPointDistanceFromPlayer > _playerData.maxItemRange;
        
        // 최대 거리 안에 있을 때
        if (!_isOverItemRange)
        {
            // TargetPoint는 플레이어의 정면상 아이템 최대 거리에 위치
            _throwTargetPoint = playerForward * hitPointDistanceFromPlayer + playerPosition;
            _throwTargetPoint.y = hit.point.y;
            
            ShowTargetPoint();
            DrawParabola();
            return;
        }

        // 최대 거리 밖에 point가 있을 때
        // TargetPoint는 플레이어로 부터 정면상으로 최대 거리에 위치
        _throwTargetPoint = playerForward * _playerData.maxItemRange + playerPosition;
        
        // 위에서 나온 _throwTargetPoint로부터 아래방향으로 Ray를 쐈을 때 hit 지점이 TargetPoint
        Ray downRay = new Ray(_throwTargetPoint, Vector3.down);
        Debug.DrawRay(_throwTargetPoint, Vector3.down * Mathf.Infinity, Color.black);
        if (!Physics.Raycast(downRay, out hit, Mathf.Infinity, layerMask))
        {
            ray = new Ray(playerPosition, cameraForward);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                _isNotHit = true;
                return;
            }

            _throwTargetPoint = Vector3.ProjectOnPlane(_throwTargetPoint, hit.normal);
            // Ray에 맞지 않는 경우는 없을것으로 예상되지만 맞지 않았다면 isNotHit을 true로 만들고 Return
        }
        
        _throwTargetPoint.y = hit.point.y;
        
        ShowTargetPoint();
        DrawParabola();
    }

    private void ShowTargetPoint()
    {
        if (_itemShowPrefabInstance == null)
        {
            _itemShowPrefabInstance = Instantiate(_itemPrefab);
            _itemShowPrefabInstance.GetComponent<Collider>().enabled = false;
            _itemShowPrefabInstance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void RemoveTargetPoint()
    {
        if (_itemShowPrefabInstance == null)
        {
            return;
        }

        Destroy(_itemShowPrefabInstance);
        _itemShowPrefabInstance = null;
    }

    private void DrawParabola(bool greaterAngle = false)
    {
        float angleInRadian = GetParabolaShootingAngleInRadian(greaterAngle);

        if (float.IsNaN(angleInRadian))
        {
            if (_prevPosition.sqrMagnitude == 0)
            {
                HandleAimingCancel();
                return;
            }

            _throwTargetPoint = _prevPosition;
            angleInRadian = GetParabolaShootingAngleInRadian(greaterAngle);
        }

        _prevPosition = _throwTargetPoint;
        
        Vector3 originPosition = _shootPoint.position;
        Vector3 targetDirection = _throwTargetPoint - originPosition;
        float targetDistance = targetDirection.magnitude;
        targetDirection.Normalize();
        
        _shootPoint.localRotation = Quaternion.Euler(-angleInRadian * Mathf.Rad2Deg, 0f, 0f);
        
        float gravity = Mathf.Abs(Physics.gravity.y);
        float v0x = _playerData.throwPower * Mathf.Cos(angleInRadian);
        float v0y = _playerData.throwPower * Mathf.Sin(angleInRadian);
        float a = -gravity / (2 * v0x * v0x);
        float tanAngle = v0y / v0x;
        
        const float DIVIDE = 0.01f;
        int count = Mathf.CeilToInt(targetDistance / DIVIDE);
        
        LineDrawHelper.Instance.SetPositionCount(count);
        Vector3[] list = new Vector3[count];
        float xPrime = 0f;
        for (int i = 0; i < count; i++)
        {
            Vector3 relativePosition = targetDirection * xPrime;
            relativePosition.y = a * xPrime * xPrime + tanAngle * xPrime;
            if (originPosition.y > _throwTargetPoint.y)
            {
                if (relativePosition.y + originPosition.y < _throwTargetPoint.y)
                {
                    count = i;
                    break;
                }
            }

            list[i] = originPosition + relativePosition;
            xPrime += DIVIDE;
        }
        
        LineDrawHelper.Instance.SetPositionCount(count);
        _itemShowPrefabInstance.transform.position = list[count - 1];
        LineDrawHelper.Instance.DrawParabola(list);
    }

    private float GetParabolaShootingAngleInRadian(bool greaterAngle)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 originPosition = _shootPoint.position; 

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
        if (_itemInHand == null || _isNotHit)
        {
            return;
        }

        Debug.Assert(_mainCamera != null, "_mainCamera != null");
        Transform cameraTransform = _mainCamera.transform;
        Vector3 playerPosTop = _shootPoint.position;
        GameObject itemPrefab =
            Instantiate(_itemInHand, playerPosTop, cameraTransform.localRotation);
        Rigidbody itemRigidbody = itemPrefab.GetComponent<Rigidbody>();
        itemPrefab.GetComponent<ItemHandler>().Init(_playerData.itemGaugeAmount, _playerData.itemImpactRadius);
        itemRigidbody.AddForce(_shootPoint.forward * _playerData.throwPower, ForceMode.VelocityChange);
        
        _itemInHand = null;
        HandleAimingCancel();
    }
}