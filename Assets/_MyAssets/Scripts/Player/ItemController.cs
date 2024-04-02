using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerItemData _itemData;
    [SerializeField] private PlayerInputData _inputData;

    [SerializeField] private GameObject _pointObject;

    private GameObject _itemInHand;
    private Camera _mainCamera;

    private bool _isOnAiming;

    private Vector3 _throwTargetPoint;

    private void Awake()
    {
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
        // Debug용 임시 입력
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _itemInHand = _itemData.items[0].item;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _itemInHand = _itemData.items[1].item;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _itemInHand = _itemData.items[2].item;
        }

        if (_isOnAiming)
        {
            SetThrowTargetPosition();
        }
        
    }

    private void HandleAiming()
    {
        _isOnAiming = true;
        PlayerMove.Instance.ChangeCameraToAiming();
    }

    private void SetThrowTargetPosition()
    {
        Transform cameraTransform = _mainCamera.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        LayerMask layerMask = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out RaycastHit hit, _playerData.throwRayDistance, layerMask))
        {
            _throwTargetPoint = hit.point;
            _pointObject.transform.position = _throwTargetPoint;
            
            return;
        }

        float angle = cameraTransform.forward.y * 100;
        Vector3 cameraPosition = _mainCamera.transform.position;
        Vector3 playerPosition = transform.position;
        
        // 각도 제한 20도
        if (angle > 19.0f)
        {
            if (angle > 19.5f)
            {
                return;
            }
            _throwTargetPoint = playerPosition + cameraTransform.forward * _playerData.maxItemRange;
            _pointObject.transform.position = _throwTargetPoint;
            return;
        }

        Vector3 direction = (playerPosition - cameraPosition).normalized;
        float directionAngle = Vector3.Angle(Vector3.down, direction);
        const float distanceFromCamera = 3.0f;
        
        // 카메라로부터 떨어진 거리 3
        // Todo 각도에 따라 도착 지점 결정
        // Todo 최대 각도를 넘을 경우 값 고정
        // Todo 최소 각도 혹은 최소 거리를 지정
        // Todo 포물선 실제 동작
    }

    private void HandleAimingCancel()
    {
        _isOnAiming = false;
        PlayerMove.Instance.ChangeCameraToFreeLook();
    }

    private void HandleShoot()
    {
        if (_itemInHand == null)
        {
            return;
        }

        Debug.Assert(_mainCamera != null, "_mainCamera != null");
        Vector3 direction = _mainCamera.transform.forward;
        Vector3 playerPos = transform.position;
        playerPos.y += 1.0f;
        GameObject itemPrefab =
            Instantiate(_itemInHand, playerPos + direction, _mainCamera.transform.localRotation);
        Rigidbody rigidbody = itemPrefab.GetComponent<Rigidbody>();

        direction.Normalize();
        rigidbody.AddForce(direction * _playerData.throwPower, ForceMode.VelocityChange);
        _itemInHand = null;
    }
}