using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WirePlayer : PlayerMove
{
    [SerializeField] private PlayerWireData _myData;
    [SerializeField] private GameObject _wireAvailableUI;

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
            return;
        }

        _wireAvailableUI.SetActive(CanHangWire);
        
        base.Update();
    }

    public void OnWireButtonClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
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
        const float TOLERANCE = 0.1f;
        
        LineDraw.Instance.TurnOnLine();
        
        while (t <= _myData.wireActionDuration && !IsCollideWhenWireAction)
        {
            float alpha = t / _myData.wireActionDuration;
 //            transform.position = Vector3.Lerp(initPos, _targetPosition,
 //                alpha < 0.5f ? 4.0f * alpha * alpha * alpha : 1.0f - Mathf.Pow(-2.0f * alpha + 2.0f, 3.0f) / 2.0f);
 
 //            if (alpha == 0.0f)
 //            {
 //                alpha = 0;
 //            }
 //            else if(Math.Abs(alpha - 1.0f) < TOLERANCE)
 //            {
 //                alpha = 1;
 //            }
 //            else
 //            {
 //                alpha = alpha < 0.5f
 //                    ? Mathf.Pow(2.0f, 20.0f * alpha - 10.0f) / 2.0f
 //                    : (2.0f - Mathf.Pow(2.0f, -20.0f * alpha + 10.0f)) / 2.0f;
 //            }
 //
 //            transform.position = Vector3.Lerp(initPos, _targetPosition, alpha);

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
