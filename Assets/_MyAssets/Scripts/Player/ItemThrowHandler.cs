using System.Collections;
using Cinemachine;
using UnityEngine;

public class ItemThrowHandler : Singleton<ItemThrowHandler>
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerInputData _inputData;
    
    private GameObject _itemPrefab;
    private GameObject _itemShowPrefab;

    public bool IsItemOnHand { get; private set; }
    
    private Camera _mainCamera;

    private bool _isOnAiming;
    private bool _isOverItemRange;

    private Transform _shootPoint;
    private Vector3 _throwTargetPoint;
    private Vector3 _shootDirection;
    private Vector3 _prevPosition;

    private IEnumerator _cameraBlendingRoutine;

    private void Awake()
    {
        _shootPoint = transform.Find("ShootPoint").GetComponent<Transform>();
        _itemPrefab = Resources.Load<GameObject>("Items/SoundBomb");
        _itemShowPrefab = Instantiate(Resources.Load<GameObject>("Items/TargetPointItemShow"));
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
        if (_isOnAiming)
        {
            SetThrowTargetPosition();
            LineDrawHelper.Instance.EnableLine();
            PlayerMove.Instance.AlignPlayerToCameraForward();
        }
    }

    public void GetItem()
    {
        IsItemOnHand = true;
    }

    private void HandleAiming()
    {
        if (!IsItemOnHand)
        {
            return;
        }
        
        CameraController.Instance.ChangeCameraFromFreeLookToAiming();
        PlayerMove.Instance.AlignPlayerToCameraForward();
        if (_cameraBlendingRoutine != null)
        {
            return;
        }

        _cameraBlendingRoutine = CameraBlendingRoutine();
        StartCoroutine(_cameraBlendingRoutine);
    }

    private IEnumerator CameraBlendingRoutine()
    {
        yield return new WaitForEndOfFrame();
        
        while (CameraController.Instance.IsBlending)
        {
            yield return null;
        }

        _isOnAiming = true;
    }

    private void HandleAimingCancel()
    {
        if (CameraController.Instance.BrainCamera.ActiveVirtualCamera as CinemachineFreeLook != CameraController.Instance.AimingCamera)
        {
            return;
        }
        _isOnAiming = false;
        CameraController.Instance.ChangeCameraFromAimingToFreeLook();
        LineDrawHelper.Instance.DisableLine();
        RemoveTargetPoint();

        if (_cameraBlendingRoutine == null)
        {
            return;
        }

        StopCoroutine(_cameraBlendingRoutine);
        _cameraBlendingRoutine = null;
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
                                | LayerMask.GetMask("Item"));
        
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Debug.Assert(false, "Invalid Situation (ray hit)");
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
        if (!Physics.Raycast(downRay, out hit, Mathf.Infinity, layerMask))
        {
            ray = new Ray(playerPosition, cameraForward);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Assert(false, "Invalid Situation (ray hit)");
                return;
            }

            _throwTargetPoint = Vector3.ProjectOnPlane(_throwTargetPoint, hit.normal);
        }
        
        _throwTargetPoint.y = hit.point.y;
        
        ShowTargetPoint();
        DrawParabola();
    }

    private void ShowTargetPoint()
    {
        _itemShowPrefab.SetActive(true);
    }

    private void RemoveTargetPoint()
    {
        _itemShowPrefab.SetActive(false);
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
        _itemShowPrefab.transform.position = list[count - 1];
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
        if (!IsItemOnHand 
            || CameraController.Instance.BrainCamera.ActiveVirtualCamera as CinemachineFreeLook !=
            CameraController.Instance.AimingCamera)
        {
            return;
        }

        Debug.Assert(_mainCamera != null, "_mainCamera != null");
        Transform cameraTransform = _mainCamera.transform;
        Vector3 playerPosTop = _shootPoint.position;
        
        GameObject itemPrefab =
            Instantiate(_itemPrefab, playerPosTop, cameraTransform.localRotation);
        Rigidbody itemRigidbody = itemPrefab.GetComponent<Rigidbody>();
        itemPrefab.GetComponent<ItemObjectFlyHandler>().Init(_playerData.itemGaugeAmount, _playerData.itemImpactRadius);
        itemRigidbody.AddForce(_shootPoint.forward * _playerData.throwPower, ForceMode.VelocityChange);

        IsItemOnHand = false;
        HandleAimingCancel();
    }
}