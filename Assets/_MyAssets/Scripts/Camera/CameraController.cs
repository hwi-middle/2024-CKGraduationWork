using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private PlayerInputData _inputData;
    public CinemachineBrain BrainCamera { get; private set; }
    public CinemachineFreeLook FreeLookCamera { get; private set; }
    public CinemachineFreeLook AimingCamera { get; private set; }
    public CinemachineVirtualCamera InCabinetCamera { get; private set; }
    public CinemachineVirtualCamera PeekCamera { get; private set; }

    private CinemachineComposer _peekCameraComposer;

    public bool IsBlending => BrainCamera.IsBlending;

    private Camera _mainCamera;

    private IEnumerator _changeHeightRoutine;
    public bool IsOnRoutine => _changeHeightRoutine != null;

    private IEnumerator _changeCameraFromPeekToInCabinet;

    private Transform _peekPoint;
    
    [Header("Gamepad Camera 이동 속도")]
    [SerializeField] private float _gamepadFreeLookSpeed = 180.0f;

    [Header("Gamepad Camera Item Aim 이동 속도")]
    [SerializeField] private float _gamepadAimSpeed = 90.0f;

    private Vector2 _gamepadAxis;
    private Vector2 _gamepadPeekAxis;
    private bool _isGamePadAxisPressed;
    
    [Header("Peek Camera 마우스 이동 속도")]
    [SerializeField] private float _mousePeekAimSpeed = 1.0f;
    [Header("Peek Camera 게임패드 이동 속도")]
    [SerializeField] private float _gamepadPeekAimSpeed = 1.0f;
    

    [Header("Peek Range")]
    [SerializeField] private float _maxPeekRange = 0.8f;

    [Header("앉기 시 높이 및 Lerp Time")]
    [SerializeField] private float _heightOffset = 0.5f;
    [SerializeField] private float _routineDuration = 0.25f;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _inputData.mouseAxisEvent += HandleMouseAxisEvent;
        _inputData.gamepadAxisEvent += HandleCameraAxisByGamepadNormalEvent;
        _inputData.peekGamepadAxisEvent += HandleGamepadAxisPeekEvent;
    }

    private void OnDisable()
    {
        _inputData.mouseAxisEvent -= HandleMouseAxisEvent;
        _inputData.gamepadAxisEvent -= HandleCameraAxisByGamepadNormalEvent;
        _inputData.peekGamepadAxisEvent -= HandleGamepadAxisPeekEvent;
    }

    public void Start()
    {
        Debug.Assert(_mainCamera != null, "_mainCamera != null");
        
        GameObject cameras = Instantiate(Resources.Load<GameObject>("Camera/Cameras"));
        
        Debug.Assert(cameras != null, "cameras != null");

        // Camera Component Setting
        BrainCamera = _mainCamera.GetComponent<CinemachineBrain>();
        FreeLookCamera = cameras.transform.Find("FreeLook Camera").GetComponent<CinemachineFreeLook>();
        AimingCamera = cameras.transform.Find("Aiming Camera").GetComponent<CinemachineFreeLook>();
        InCabinetCamera = cameras.transform.Find("InCabinet Camera").GetComponent<CinemachineVirtualCamera>();
        PeekCamera = cameras.transform.Find("Peek Camera").GetComponent<CinemachineVirtualCamera>();
        
        Transform playerTransform = transform;
        
        // Peek Point Transform
        _peekPoint = playerTransform.Find("PeekPoint");

        // Cameras Follow & LookAt Setting
        FreeLookCamera.Follow = playerTransform;
        FreeLookCamera.LookAt = playerTransform;

        AimingCamera.Follow = playerTransform;
        AimingCamera.LookAt = playerTransform;

        Transform hideableObjectAimTransform = playerTransform.Find("InHideableObjectAim").transform;
        InCabinetCamera.Follow = playerTransform;
        InCabinetCamera.LookAt = hideableObjectAimTransform;

        PeekCamera.Follow = playerTransform;
        PeekCamera.LookAt = _peekPoint;
        _peekCameraComposer = PeekCamera.GetCinemachineComponent<CinemachineComposer>();
        
        // Live 카메라를 FreeLook으로 설정
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
        AlignCameraToPlayer();
    }

    private void Update()
    {
        RotateCameraByGamepad();
        RotatePeekCameraByGamepad();
    }

    private void HandleCameraAxisByGamepadNormalEvent(Vector2 axis, bool isPressed)
    {
        _isGamePadAxisPressed = isPressed;
        _gamepadAxis = axis;
    }

    private void RotateCameraByGamepad()
    {
        if (!_isGamePadAxisPressed)
        {
            return;
        }

        FreeLookCamera.m_XAxis.Value += _gamepadAxis.x * _gamepadFreeLookSpeed * Time.deltaTime;
        FreeLookCamera.m_YAxis.Value += -_gamepadAxis.y * Time.deltaTime;
        
        AimingCamera.m_XAxis.Value += _gamepadAxis.x * _gamepadAimSpeed * Time.deltaTime;
        AimingCamera.m_YAxis.Value += -(_gamepadAxis.y * 0.5f) * Time.deltaTime;
    }

    private void AlignCameraToPlayer()
    {
        Vector3 playerForward = transform.forward;
        float xAxis = Mathf.Atan2(playerForward.x, playerForward.z) * Mathf.Rad2Deg;
        FreeLookCamera.m_XAxis.Value = xAxis;
        FreeLookCamera.m_YAxis.Value = 0.5f;

        AimingCamera.m_XAxis.Value = xAxis;
        AimingCamera.m_YAxis.Value = 0.5f;
    }
    
    private void HandleMouseAxisEvent(float value)
    {
        if (BrainCamera.ActiveVirtualCamera.Name != PeekCamera.name)
        {
            return;
        }
        
        CalculatePeekCameraOffsetX(false, value);
    }

    private void HandleGamepadAxisPeekEvent(Vector2 axis, bool isPressed)
    {
        _isGamePadAxisPressed = isPressed;
        _gamepadPeekAxis = axis;
    }

    private void RotatePeekCameraByGamepad()
    {
        if (!_isGamePadAxisPressed || BrainCamera.ActiveVirtualCamera.Name != PeekCamera.name)
        {
            return;
        }

        CalculatePeekCameraOffsetX(true);
    }

    private void CalculatePeekCameraOffsetX(bool isGamepad, float value = 0)
    {
        float offsetX = _peekCameraComposer.m_TrackedObjectOffset.x;
        float speed = isGamepad ? _gamepadPeekAimSpeed * Time.deltaTime : _mousePeekAimSpeed * Time.deltaTime;

        bool isLeftAxis = isGamepad ? _gamepadPeekAxis.x < 0 : value < 0;

        offsetX += isLeftAxis ? -speed : speed;

        bool isOverRange = Mathf.Abs(offsetX) > _maxPeekRange;

        if (isOverRange)
        {
            offsetX = isLeftAxis ? -_maxPeekRange : _maxPeekRange;
            _peekCameraComposer.m_TrackedObjectOffset.x = offsetX;
            return;
        }

        _peekCameraComposer.m_TrackedObjectOffset.x = offsetX;
    }

    public void ToggleCrouchCameraHeight(bool isCrouch)
    {
        if (IsOnRoutine)
        {
            return;
        }

        _changeHeightRoutine = ToggleCrouchCameraHeightRoutine(isCrouch);
        StartCoroutine(_changeHeightRoutine);
    }

    private IEnumerator ToggleCrouchCameraHeightRoutine(bool isCrouch)
    {
        // Camera Orbit Length (FreeLook Length로 Aiming과 공유)
        float orbitLength = FreeLookCamera.m_Orbits.Length;
        
        // 앉기 상태에 따라 Offset의 증감 결정
        float offsetDelta = isCrouch ? _heightOffset : -_heightOffset;
        
        // FreeLook Camera values 
        List<float> freeLookStartHeights = new();
        List<float> freeLookTargetHeights = new();
        List<float> freeLookStartYOffsets = new();
        List<float> freeLookTargetYOffsets = new();
        
        // Aiming Camera Values
        List<float> aimingStartHeights = new();
        List<float> aimingTargetHeights = new();
        List<float> aimingStartYOffsets = new();
        List<float> aimingTargetYOffsets = new();

        for (int i = 0; i < orbitLength; i++)
        {
            // Free Look Camera Init
            CinemachineComposer freeLookComposer = FreeLookCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            freeLookStartHeights.Add(FreeLookCamera.m_Orbits[i].m_Height);
            freeLookTargetHeights.Add(FreeLookCamera.m_Orbits[i].m_Height - offsetDelta); 
            
            freeLookStartYOffsets.Add(freeLookComposer.m_TrackedObjectOffset.y);
            freeLookTargetYOffsets.Add(freeLookComposer.m_TrackedObjectOffset.y - offsetDelta);
            
            // Aiming Camera Init
            CinemachineComposer aimingComposer = AimingCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            aimingStartHeights.Add(AimingCamera.m_Orbits[i].m_Height);
            aimingTargetHeights.Add(AimingCamera.m_Orbits[i].m_Height - offsetDelta);
            
            aimingStartYOffsets.Add(aimingComposer.m_TrackedObjectOffset.y);
            aimingTargetYOffsets.Add(aimingComposer.m_TrackedObjectOffset.y - offsetDelta);
        }

        float t = 0;
        while (t <= _routineDuration)
        {
            float alpha = t / _routineDuration;

            for (int i = 0; i < orbitLength; i++)
            {
                // FreeLook Camera Lerp Value
                CinemachineComposer freeLookComposer = FreeLookCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
                FreeLookCamera.m_Orbits[i].m_Height = Mathf.Lerp(freeLookStartHeights[i], freeLookTargetHeights[i], alpha);
                freeLookComposer.m_TrackedObjectOffset.y = Mathf.Lerp(freeLookStartYOffsets[i], freeLookTargetYOffsets[i], alpha);

                // Aiming Camera Lerp value
                CinemachineComposer aimingComposer =
                    AimingCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
                AimingCamera.m_Orbits[i].m_Height = Mathf.Lerp(aimingStartHeights[i], aimingTargetHeights[i], alpha);
                aimingComposer.m_TrackedObjectOffset.y =
                    Mathf.Lerp(aimingStartYOffsets[i], aimingTargetYOffsets[i], alpha);
            }
            
            yield return null;
            t += Time.deltaTime;
        }

        for (int i = 0; i < orbitLength; i++)
        {
            // FreeLook Camera Last Set
            CinemachineComposer freeLookComposer = FreeLookCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            FreeLookCamera.m_Orbits[i].m_Height = freeLookTargetHeights[i];
            freeLookComposer.m_TrackedObjectOffset.y = freeLookTargetYOffsets[i];
            
            // Aiming Camera Last Set
            CinemachineComposer aimingComposer = AimingCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            AimingCamera.m_Orbits[i].m_Height = aimingTargetHeights[i];
            aimingComposer.m_TrackedObjectOffset.y = aimingTargetYOffsets[i];
        }

        _changeHeightRoutine = null;
    }

    public void ChangeCameraFromAimingToFreeLook()
    {
        FreeLookCamera.MoveToTopOfPrioritySubqueue();

        FreeLookCamera.m_XAxis.Value = AimingCamera.m_XAxis.Value;
        FreeLookCamera.m_YAxis.Value = 0.5f;
    }

    public void ChangeCameraFromFreeLookToAiming()
    {
        AimingCamera.MoveToTopOfPrioritySubqueue();

        AimingCamera.m_XAxis.Value = FreeLookCamera.m_XAxis.Value;
        AimingCamera.m_YAxis.Value = 0.5f;
    }

    public void ChangeCameraFromFreeLookToInCabinet()
    {
        InCabinetCamera.MoveToTopOfPrioritySubqueue();
        BrainCamera.m_DefaultBlend.m_Time = 0.5f;
    }

    public void ChangeCameraFromPeekToInCabinet()
    {
        if (_changeCameraFromPeekToInCabinet != null)
        {
            return;
        }
        
        InCabinetCamera.MoveToTopOfPrioritySubqueue();
    }

    public void ChangeCameraFromCabinetToFreeLook()
    {
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
        FreeLookCamera.m_YAxis.Value = 0.5f;
    }

    public void ChangeCameraToPeek()
    {
        if (_changeCameraFromPeekToInCabinet != null)
        {
            return;
        }

        // PeekCamera.m_XAxis.Value = 0;
        PeekCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.x = 0;
        PeekCamera.MoveToTopOfPrioritySubqueue();
    }
}
