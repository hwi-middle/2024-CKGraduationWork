using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private float _blendingDuration = 0.5f;
    public CinemachineBrain BrainCamera { get; private set; }
    public CinemachineFreeLook FreeLookCamera { get; private set; }
    public CinemachineFreeLook AimingCamera { get; private set; }
    public CinemachineVirtualCamera InCabinetCamera { get; private set; }
    public CinemachineVirtualCamera PeekCamera { get; private set; }
    public CinemachineVirtualCamera CubeCamera { get; private set; }
    public CinemachineVirtualCamera AssassinateCamera { get; private set; }

    private CinemachineComposer _peekCameraComposer;

    public bool IsBlending => BrainCamera.IsBlending;

    private Camera _mainCamera;

    private IEnumerator _changeHeightRoutine;
    public bool IsOnChangeHeightRoutine => _changeHeightRoutine != null;

    private IEnumerator _changeCameraFromPeekToInCabinet;

    [Header("Peek Point Transform")]
    [SerializeField] private Transform _peekPoint;
    
    [Header("Peek Camera 이동 속도")]
    [SerializeField] private float _aimSpeed = 1.0f;

    [Header("Peek Range")]
    [SerializeField] private float _maxPeekRange = 0.8f;

    [Header("앉기 시 높이 및 Lerp Time")]
    [SerializeField] private float _heightOffset = 0.5f;
    [SerializeField] private float _routineDuration = 0.25f;

    public void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void OnEnable()
    {
        _inputData.mouseAxisEvent += HandleMouseAxisEvent;
    }

    public void Start()
    {
        Debug.Assert(_mainCamera != null, "_mainCamera != null");
        
        GameObject virtualCameras = Instantiate(Resources.Load<GameObject>("Camera/VirtualCameras"));
        
        Debug.Assert(virtualCameras != null, "virtualCameras != null");

        // Camera Component Setting
        BrainCamera = _mainCamera.GetComponent<CinemachineBrain>();
        FreeLookCamera = virtualCameras.transform.GetChild(0).GetComponent<CinemachineFreeLook>();
        AimingCamera = virtualCameras.transform.GetChild(1).GetComponent<CinemachineFreeLook>();
        InCabinetCamera = virtualCameras.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();
        PeekCamera = virtualCameras.transform.GetChild(3).GetComponent<CinemachineVirtualCamera>();
        CubeCamera = virtualCameras.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>();
        AssassinateCamera = virtualCameras.transform.GetChild(5).GetComponent<CinemachineVirtualCamera>();
        
        // Brain Camera Blending Duration Setting
        BrainCamera.m_DefaultBlend.m_Time = _blendingDuration;
        
        Transform playerTransform = transform;

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

        // 상호작용 시 설정
        CubeCamera.Follow = null;
        CubeCamera.LookAt = null;
        AssassinateCamera.Follow = null;
        AssassinateCamera.LookAt = null;
        
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
    }
    
    private void HandleMouseAxisEvent(float value)
    {
        if (BrainCamera.ActiveVirtualCamera.Name != PeekCamera.name)
        {
            return;
        }
        
        float offsetX = _peekCameraComposer.m_TrackedObjectOffset.x;
        float speed = _aimSpeed * Time.deltaTime;
        
        // Mouse Axis의 좌 우 확인
        bool isLeftAxis = value < 0;

        // offset 연산
        offsetX += isLeftAxis ? -speed : speed;
        
        // 계산 된 Offset이 최대 범위를 벗어나는지 확인

        bool isOverRange = Mathf.Abs(offsetX) > _maxPeekRange;

        if (isOverRange)
        {
            // Offset을 최대 범위 값에 맞춤
            offsetX = isLeftAxis ? -_maxPeekRange : _maxPeekRange;
            _peekCameraComposer.m_TrackedObjectOffset.x = offsetX;
            return;
        }

        // 계산 된 Offset을 카메라에 적용
        _peekCameraComposer.m_TrackedObjectOffset.x = offsetX;
    }

    public void ToggleCrouchCameraHeight(bool isCrouch)
    {
        if (IsOnChangeHeightRoutine)
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

    /// <summary>
    /// 아이템(에임) 카메라 에서 자유 카메라로 변경합니다.
    /// </summary>
    public void ChangeCameraFromAimingToFreeLook()
    {
        FreeLookCamera.MoveToTopOfPrioritySubqueue();

        FreeLookCamera.m_XAxis.Value = AimingCamera.m_XAxis.Value;
        FreeLookCamera.m_YAxis.Value = 0.5f;
    }

    /// <summary>
    /// 자유 카메라에서 아이템(에임) 카메라로 변경합니다.
    /// </summary>
    public void ChangeCameraFromFreeLookToAiming()
    {
        AimingCamera.MoveToTopOfPrioritySubqueue();

        AimingCamera.m_XAxis.Value = FreeLookCamera.m_XAxis.Value;
        AimingCamera.m_YAxis.Value = 0.5f;
    }

    /// <summary>
    /// 숨기 상호작용 시 카메라를 Cabinet 카메라로 변경합니다.
    /// </summary>
    public void ChangeCameraFromFreeLookToInCabinet()
    {
        InCabinetCamera.MoveToTopOfPrioritySubqueue();
        BrainCamera.m_DefaultBlend.m_Time = 0.5f;
    }

    /// <summary>
    /// 숨기 상호작용 중 Peek 카메라에서 Cabinet 카메라로 변경합니다.
    /// </summary>
    public void ChangeCameraFromPeekToInCabinet()
    {
        if (_changeCameraFromPeekToInCabinet != null)
        {
            return;
        }
        
        InCabinetCamera.MoveToTopOfPrioritySubqueue();
    }

    /// <summary>
    /// 숨기 상호작용 종료 시 자유 카메라로 변경합니다.
    /// </summary>
    public void ChangeCameraFromCabinetToFreeLook()
    {
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
        FreeLookCamera.m_YAxis.Value = 0.5f;
    }

    /// <summary>
    /// 숨기 상호작용 중 Peek 카메라로 변경합니다.
    /// </summary>
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

    /// <summary>
    /// 큐브 상호작용 시 큐브 카메라로 변경합니다.
    /// </summary>
    /// <param name="follow">큐브 연출 지점의 Transform</param>
    /// <param name="lookAt">큐브 Transform</param>
    public void ChangeCameraToCube(Transform follow, Transform lookAt)
    {
        CubeCamera.Follow = follow;
        CubeCamera.LookAt = lookAt;
        CubeCamera.MoveToTopOfPrioritySubqueue();
    }
    
    /// <summary>
    /// 큐브 상호작용 종료 시 자유 카메라로 변경합니다.
    /// </summary>
    public void ChangeCameraFromCubeToFreeLook()
    {
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
        CubeCamera.Follow = null;
        CubeCamera.LookAt = null;
    }

    /// <summary>
    /// 암살 상호작용 시 암살 카메라로 변경합니다.
    /// </summary>
    /// <param name="follow">암살 대상 연출 지점의 Transform</param>
    /// <param name="lookAt">암살 대상 Transform</param>
    public void ChangeCameraToAssassinate(Transform follow, Transform lookAt)
    {
        AssassinateCamera.Follow = follow;
        AssassinateCamera.LookAt = lookAt;
        AssassinateCamera.MoveToTopOfPrioritySubqueue();
    }

    /// <summary>
    /// 암살 상호작용 종료 시 자유 카메라로 변경합니다.
    /// </summary>
    public void ChangeCameraFromAssassinateFromFreeLook()
    {
        AssassinateCamera.Follow = null;
        AssassinateCamera.LookAt = null;
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
    }
}
