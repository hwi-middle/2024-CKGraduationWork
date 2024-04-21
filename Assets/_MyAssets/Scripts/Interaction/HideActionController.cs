using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class HideActionController : Singleton<HideActionController>
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private PlayerData _data;

    private Transform _playerTransform;
    
    private bool _isCrouch;
    private bool _isInHideableObject;

    private GameObject _currentHideableObject;
    private float _exitDistance;
    private Vector3 _currentHideableObjectForward;

    private bool IsOnRoutine => _hideActionRoutine != null || _hideExitActionRoutine != null;
    private IEnumerator _hideActionRoutine;
    private IEnumerator _hideExitActionRoutine;
    
    private Camera _mainCamera; 
    private GameObject _peekCamera;

    private void OnEnable()
    {
        _inputData.peekEvent += HandlePeekAction;
        _inputData.hideExitEvent += HandleHideExitAction;
        _inputData.peekExitEvent += HandlePeekExitAction;
    }

    private void OnDisable()
    {
        _inputData.peekEvent -= HandlePeekAction;
        _inputData.hideExitEvent -= HandleHideExitAction;
        _inputData.peekExitEvent -= HandlePeekExitAction;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _playerTransform = transform;
    }

    private void Update()
    {
        IsPlayerInHideableZone();
    }

    private void IsPlayerInHideableZone()
    {
        if (_isInHideableObject || CoverUpZoneController.hideableZoneCount == 0)
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
        CameraController.Instance.ChangeCameraToPeek();
        PlayerMove.Instance.AddPlayerState(EPlayerState.Peek);
    }

    private void HandlePeekExitAction()
    {
        CameraController.Instance.ChangeCameraFromPeekToInCabinet();
        PlayerMove.Instance.RemovePlayerState(EPlayerState.Peek);
    }

    private void HandleHideExitAction()
    {
        if (PlayerMove.Instance.CheckPlayerState(EPlayerState.Peek) || IsOnRoutine)
        {
            return;
        }
        
        _isInHideableObject = false;
        _hideExitActionRoutine = HideExitRoutine();
        StartCoroutine(_hideExitActionRoutine);
    }

    private IEnumerator HideExitRoutine()
    {
        CameraController.Instance.ChangeCameraFromCabinetToFreeLook();
        Vector3 startPosition = _playerTransform.position;
        Vector3 exitPoint = _playerTransform.forward.normalized * _exitDistance + startPosition;

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

    public bool IsInFrontOfHideableObject(Transform hitObject)
    {
        Vector3 playerPosition = Vector3.ProjectOnPlane(_playerTransform.position, Vector3.up);
        Vector3 hitObjectPosition = Vector3.ProjectOnPlane(hitObject.position, Vector3.up);
        Vector3 objectForward = hitObject.forward;
        Vector3 playerDirectionFromObject = (playerPosition - hitObjectPosition).normalized;

        float angle = Vector3.Angle(objectForward, playerDirectionFromObject);

        return angle < 45.0f || Mathf.Approximately(angle, 45.0f);
    }

    public void HideAction()
    {
        if (IsOnRoutine)
        {
            return;
        }
        
        Transform cameraTransform = _mainCamera.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        LayerMask layerMask = LayerMask.GetMask("Hideable");

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return;
        }
        
        _isCrouch = PlayerMove.Instance.CheckPlayerState(EPlayerState.Crouch);
        PlayerMove.Instance.SetInitState();
        _isInHideableObject = true;
        PlayerMove.Instance.AddPlayerState(EPlayerState.Hide);
        
        Vector3 playerOnPlane = Vector3.ProjectOnPlane(_playerTransform.position, Vector3.up);
        Vector3 hideableObjectOnPlane = Vector3.ProjectOnPlane(hit.transform.position, Vector3.up);
        _currentHideableObjectForward = hit.normal;
        
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

        Vector3 startPosition = _playerTransform.position;
        Quaternion startRotation = _playerTransform.rotation;
        
        Vector3 targetPosition = _currentHideableObject.transform.position;
        targetPosition.y = startPosition.y;

        Quaternion targetRotation = Quaternion.LookRotation(_currentHideableObjectForward);
        
        targetRotation.x = 0f;
        targetRotation.z = 0f;

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
