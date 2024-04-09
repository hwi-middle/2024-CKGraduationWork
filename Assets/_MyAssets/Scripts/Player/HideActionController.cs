using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class HideActionController : Singleton<HideActionController>
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private PlayerData _data;

    private Vector3 _exitPoint;

    public bool IsOnAction => _hideActionRoutine != null || _hideExitActionRoutine != null;

    public static bool isHiding = false;

    private IEnumerator _hideActionRoutine;
    private IEnumerator _hideExitActionRoutine;
    
    private GameObject _currentHideableObject;

    private int _prevState;

    private Camera _mainCamera; 

    private void OnEnable()
    {
        _inputData.hideEvent += HandleHideAction;
        _inputData.peekEvent += HandlePeekAction;
        _inputData.hideExitEvent += HandleHideExitAction;
        _inputData.peekExitEvent += HandlePeekExitAction;
    }

    private void OnDisable()
    {
        _inputData.hideEvent -= HandleHideAction;
        _inputData.peekEvent -= HandlePeekAction;
        _inputData.hideExitEvent -= HandleHideExitAction;
        _inputData.peekExitEvent -= HandlePeekExitAction;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void HandlePeekAction()
    {
        CinemachineFreeLook peekCamera = _currentHideableObject.GetComponentInChildren<CinemachineFreeLook>();
        peekCamera.MoveToTopOfPrioritySubqueue();
        PlayerMove.Instance.AddPlayerState(EPlayerState.Peek);
    }

    private void HandlePeekExitAction()
    {
        PlayerMove.Instance.InCabinetCamera.MoveToTopOfPrioritySubqueue();
        PlayerMove.Instance.RemovePlayerState(EPlayerState.Peek);
    }

    private void HandleHideExitAction()
    {
        _hideExitActionRoutine = HideExitRoutine();
        StartCoroutine(_hideExitActionRoutine);
    }

    private IEnumerator HideExitRoutine()
    {
        Vector3 startPosition = transform.position;

        float t = 0;
        const float DURATION = 1.0f;

        while (t <= DURATION)
        {
            float alpha = t / DURATION;
            transform.position = Vector3.Lerp(startPosition, _exitPoint, alpha);
            yield return null;
            t += Time.deltaTime;
        }

        transform.position = _exitPoint;
        
        _hideExitActionRoutine = null;
        isHiding = false;
        PlayerMove.Instance.RestoreState(_prevState);
        PlayerMove.Instance.ChangeCameraToFreeLook();
    }

    private void HandleHideAction()
    {
        _prevState = PlayerMove.Instance.CurrentState;
        
        PlayerMove.Instance.SetInitState();
        PlayerMove.Instance.AddPlayerState(EPlayerState.Hide);
        
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
        isHiding = true;
        PlayerMove.Instance.ChangeCameraToHideAiming();

        Vector3 startPosition = transform.position;
        _exitPoint = startPosition;
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
}
