using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideActionController : Singleton<HideActionController>
{
    [SerializeField] private bool _toggleAorB;
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private PlayerData _data;

    private Vector3 _exitPoint;
    
    public bool IsOnAction => _hideActionRoutine != null && _hideExitActionRoutine != null;
    public bool IsHiding { get; private set; }

    private IEnumerator _hideActionRoutine;
    private IEnumerator _hideExitActionRoutine;
    
    private GameObject _currentHideableObject;

    private Camera _mainCamera;
    

    private void OnEnable()
    {
        _inputData.hideEvent += HandleHideAction;
    }

    private void OnDisable()
    {
        _inputData.hideEvent -= HandleHideAction;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        
    }

    public void ExitFromHideableObject()
    {
        _hideExitActionRoutine = HideExitRoutine();
        StartCoroutine(_hideExitActionRoutine);
    }

    private IEnumerator HideExitRoutine()
    {
        if (!_toggleAorB)
        {
            PlayerMove.Instance.ChangeCameraToFreeLook();
        }
        else
        {
            StartCoroutine(FOVReduceRoutine());
        }

        Vector3 startPosition = transform.position;
        Vector3 exitPosition =
            _currentHideableObject.transform.Find("ExitPoint").transform.position;
        exitPosition.y = startPosition.y;

        float t = 0;
        const float DURATION = 1.0f;

        while (t <= DURATION)
        {
            float alpha = t / DURATION;
            transform.position = Vector3.Lerp(startPosition, exitPosition, alpha);
            yield return null;
            t += Time.deltaTime;
        }

        transform.position = exitPosition;
        
        _hideExitActionRoutine = null;
        IsHiding = false;
    }

    private void HandleHideAction()
    {
        Transform cameraTransform = _mainCamera.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        LayerMask layerMask = LayerMask.GetMask("Hideable");
        
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return; 
        }

        Vector3 playerOnPlane = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        Vector3 hideableObjectOnPlane = Vector3.ProjectOnPlane(hit.transform.position, Vector3.up);
        float hitDistance = Vector3.Distance(playerOnPlane, hideableObjectOnPlane);
        
        if (hitDistance > _data.maxDistanceHideableObject)
        {
            return;
        }

        if (_hideActionRoutine != null)
        {
            return;
        }

        _currentHideableObject = hit.transform.gameObject;

        _hideActionRoutine = HideActionRoutine();
        StartCoroutine(_hideActionRoutine);
    }

    private IEnumerator HideActionRoutine()
    {
        IsHiding = true;
        if (!_toggleAorB)
        {
            PlayerMove.Instance.ChangeCameraToHideAiming();
        }
        else
        {
            StartCoroutine(FOVExpandRoutine());
        }

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        
        Vector3 targetPosition = _currentHideableObject.transform.position;
        targetPosition.y = startPosition.y;
        Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
        
        float t = 0;
        const float DURATION = 1.0f;
        while (t <= DURATION)
        {
            float alpha = t / DURATION;
            transform.position = Vector3.Lerp(startPosition, targetPosition, alpha);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, alpha);
            yield return null;
            t += Time.deltaTime;
        }

        transform.position = targetPosition;
        _hideActionRoutine = null;
    }

    private IEnumerator FOVExpandRoutine()
    {
        float t = PlayerMove.Instance.FreeLookCamera.m_Lens.FieldOfView;
        const float MAX_FOV = 100.0f;
        while (t <= MAX_FOV)
        {
            PlayerMove.Instance.FreeLookCamera.m_Lens.FieldOfView = t;
            yield return null;
            t += MAX_FOV * Time.deltaTime;
        }
        
        PlayerMove.Instance.FreeLookCamera.m_Lens.FieldOfView = MAX_FOV;
    }

    private IEnumerator FOVReduceRoutine()
    {
        float t = PlayerMove.Instance.FreeLookCamera.m_Lens.FieldOfView;
        const float MIN_FOV = 70.0f;
        float duration = t - MIN_FOV;
        while (t >= MIN_FOV)
        {
            PlayerMove.Instance.FreeLookCamera.m_Lens.FieldOfView = t;
            yield return null;
            t -= duration * Time.deltaTime;
        }

        PlayerMove.Instance.FreeLookCamera.m_Lens.FieldOfView = MIN_FOV;
    }
}
