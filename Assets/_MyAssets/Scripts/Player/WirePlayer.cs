using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WirePlayer : PlayerMove
{
    [Header("Player Data (ScriptableObject)")]
    [SerializeField] private PlayerWireData _myData;
    
    [Header("WirePoint Variable")]
    [SerializeField] private GameObject _wireAvailableUI;
    [SerializeField] private RectTransform _wireAvilableUITransform;

    private enum ECalculateType
    {
        V2,
        V3
    }

    private IEnumerator _wireAction;

    private Camera _mainCamera;

    private Vector3 _targetPosition;
    private Vector3 _wireHangPosition;

    private static readonly Vector3 CAMERA_CENTER_POINT = new(0.5f, 0.5f, 0.0f);
    
    private bool _isOnWire;

    private bool CanHangWire
    {
        get
        {
            Debug.Assert(_mainCamera != null, "_mainCamera != null");
            Ray ray = _mainCamera.ViewportPointToRay(CAMERA_CENTER_POINT);
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
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
            
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

    private void Start()
    {
        _mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void Update()
    {
        if (_isOnWire)
        {
            return;
        }
        
        ShowWirePointUI();
        
        base.Update();
    }

    private float CalculateDistance(Vector3 origin, Vector3 target, ECalculateType type)
    {
        return type == ECalculateType.V3 ? (origin - target).magnitude : ((Vector2)origin - (Vector2)target).magnitude;
    }

    private GameObject FindWirePoint()
    {
        List<GameObject> wirePointInScreen = new();
        GameObject[] allWireObjects = GameObject.FindGameObjectsWithTag("WirePoint");
        
        Debug.Assert(allWireObjects.Length != 0, "allWireObjects.Length != 0");

        foreach (var wirePoint in allWireObjects)
        {
            Vector3 myViewportPosition = _mainCamera.WorldToViewportPoint(wirePoint.transform.position);
            float distance = CalculateDistance(myViewportPosition, CAMERA_CENTER_POINT, ECalculateType.V2);
            
            const float TOLERANCE_RANGE = 0.2f;
            if (distance > TOLERANCE_RANGE)
            {
                continue;   
            }
            
            wirePointInScreen.Add(wirePoint);
        }

        Vector2 aimPoint = CAMERA_CENTER_POINT;
        
        float nearDistanceFromCenter = 2.0f;
        GameObject nearWirePoint = null;

        foreach (var wirePoint in wirePointInScreen)
        {
            float distanceFromPlayer = CalculateDistance(transform.position, wirePoint.transform.position, ECalculateType.V3);
            float distanceFromCenter = CalculateDistance(aimPoint,
                _mainCamera.WorldToViewportPoint(wirePoint.transform.position), ECalculateType.V2);

            if (distanceFromPlayer > _myData.maxWireDistance || distanceFromPlayer < _myData.minWireDistance)
            {
                continue;
            }

            if (nearDistanceFromCenter < distanceFromCenter)
            {
                continue;
            }

            nearDistanceFromCenter = distanceFromCenter;
            nearWirePoint = wirePoint;
        }

        return nearWirePoint;
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

        Vector3 wireScreenPoint = _mainCamera.WorldToScreenPoint(wirePoint.transform.position);

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
        RotatePlayer();
        _wireAction = WireActionRoutine();
        StartCoroutine(_wireAction);
    }

    private void RotatePlayer()
    {
        Debug.Assert(_mainCamera != null, "_mainCamera != null");

        Quaternion cameraRotation = _mainCamera.transform.localRotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, 1.0f); 
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
}
