using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class HideActionController : Singleton<HideActionController>
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private PlayerData _data;
    
    private bool _isCrouch;
    private bool _isInHideableObject;

    private GameObject _currentHideableObject;
    private float _exitDistance;


    private IEnumerator _hideActionRoutine;
    private IEnumerator _hideExitActionRoutine;
    
    private Camera _mainCamera; 
    private GameObject _peekCamera;

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

    private void Update()
    {
        IsPlayerInHideableZone();
    }

    private void IsPlayerInHideableZone()
    {
        if (_isInHideableObject || HideableZoneHandler.hideableZoneCount == 0)
        {
            return;
        }

        if (!PlayerMove.Instance.CheckPlayerState(EPlayerState.Crouch))
        {
            Debug.Assert(!_isInHideableObject, "_isInHideableObject Is Not True");
            
            PlayerMove.Instance.RemovePlayerState(EPlayerState.Hide);
            return;
        }
        
        PlayerMove.Instance.AddPlayerState(EPlayerState.Hide);
    }

    private void HandlePeekAction()
    {
        CameraController.Instance.ChangeCameraToPeek(_currentHideableObject);
        PlayerMove.Instance.AddPlayerState(EPlayerState.Peek);
    }

    private void HandlePeekExitAction()
    {
        CameraController.Instance.ChangeCameraFromPeekToInCabinet();
        PlayerMove.Instance.RemovePlayerState(EPlayerState.Peek);
    }

    private void HandleHideExitAction()
    {
        _isInHideableObject = false;
        _hideExitActionRoutine = HideExitRoutine();
        StartCoroutine(_hideExitActionRoutine);
    }

    private IEnumerator HideExitRoutine()
    {
        CameraController.Instance.ChangeCameraFromCabinetToFreeLook();
        Vector3 startPosition = transform.position;
        Vector3 exitPoint = transform.forward.normalized * _exitDistance + startPosition;

        float t = 0;
        const float DURATION = 1.0f;

        while (t <= DURATION)
        {
            float alpha = t / DURATION;
            transform.position = Vector3.Lerp(startPosition, exitPoint, alpha);
            yield return null;
            t += Time.deltaTime;
        }

        transform.position = exitPoint;
        
        _hideExitActionRoutine = null;

        _currentHideableObject.GetComponent<Collider>().isTrigger = false;
        
        PlayerMove.Instance.ExitHideState(_isCrouch);
        PlayerInputData.ChangeInputMap(PlayerInputData.EInputMap.PlayerAction);
    }

    private void HandleHideAction()
    {
        _isCrouch = PlayerMove.Instance.CheckPlayerState(EPlayerState.Crouch);
        PlayerMove.Instance.SetInitState();
        _isInHideableObject = true;
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
        _currentHideableObject.GetComponent<Collider>().isTrigger = true;

        _hideActionRoutine = HideActionRoutine();
        StartCoroutine(_hideActionRoutine);
    }

    private IEnumerator HideActionRoutine()
    {
        CameraController.Instance.ChangeCameraFromFreeLookToInCabinet();
        PlayerInputData.ChangeInputMap(PlayerInputData.EInputMap.HideAction);

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        
        Vector3 targetPosition = _currentHideableObject.transform.position;
        targetPosition.y = startPosition.y;
        Quaternion targetRotation = Quaternion.Euler(0, 180, 0);

        _exitDistance = Vector3.Distance(startPosition, targetPosition);
        
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
