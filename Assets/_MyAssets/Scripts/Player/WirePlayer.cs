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

            if (!isHit || !hit.transform.CompareTag("WirePoint") ||
                (hit.point - ray.origin).magnitude > _myData.maxWireDistance ||
                (hit.point - ray.origin).magnitude < _myData.minWireDistance)
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
            _wireAvailableUI.SetActive(false);
            return;
        }
        
        ShowWirePointUI();
        
        base.Update();
    }

    private GameObject FindWirePoint()
    {
        List<GameObject> wirePointInScreen = new();
        GameObject[] allWireObjects = GameObject.FindGameObjectsWithTag("WirePoint");

        foreach (var wirePoint in allWireObjects)
        {
            Vector3 myScreenPosition = _mainCamera.WorldToViewportPoint(wirePoint.transform.position);

            if (myScreenPosition.x < 0 || myScreenPosition.x > 1
                                       || myScreenPosition.y < 0 || myScreenPosition.y > 1
                                       || myScreenPosition.z < 0)
            {
                continue;
            }
            
            wirePointInScreen.Add(wirePoint);
        }

        float nearDistance = _myData.maxWireDistance;

        GameObject nearWirePoint = null;
        
        foreach(var wirePoint in wirePointInScreen)
        {
            float distance = (transform.position - wirePoint.transform.position).magnitude;

            if (distance > nearDistance || distance < _myData.minWireDistance)
            {
                continue;
            }

            nearDistance = distance;
            nearWirePoint = wirePoint;
        }

        return nearWirePoint;
    }

    private void ShowWirePointUI()
    {
        Debug.Assert(_wireAvilableUITransform != null);

        GameObject wirePoint = FindWirePoint();

        if (wirePoint == null)
        {
            _wireAvailableUI.SetActive(false);
            return;
        }
        
        
        _wireAvailableUI.SetActive(true);

        Vector3 myScreenPoint = _mainCamera.WorldToScreenPoint(wirePoint.transform.position);

        const float OFFSET = 50.0f;

        myScreenPoint.x += OFFSET;

        _wireAvilableUITransform.position = myScreenPoint;
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
