using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private float _blendingDuration = 0.5f;
    public CinemachineBrain BrainCamera { get; private set; }
    private CinemachineFreeLook FollowCamera { get; set; }
    public CinemachineFreeLook AimingCamera { get; set; }
    private CinemachineVirtualCamera InCabinetCamera { get; set; }
    private CinemachineVirtualCamera PeekCamera { get; set; }
    private CinemachineVirtualCamera CubeCamera { get; set; }

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

    public void OnEnable()
    {
        _inputData.mouseAxisEvent += HandleMouseAxisEvent;
    }

    public void Start()
    {
        _mainCamera = Camera.main;
        Debug.Assert(_mainCamera != null, "_mainCamera != null");
        
        GameObject virtualCameras = Instantiate(Resources.Load<GameObject>("Camera/VirtualCameras"));
        
        Debug.Assert(virtualCameras != null, "virtualCameras != null");

        // Camera Component Setting
        // Main Camera
        BrainCamera = _mainCamera.GetComponent<CinemachineBrain>();
        
        // Follow Camera
        FollowCamera = virtualCameras.transform.Find("Follow Camera").GetComponent<CinemachineFreeLook>();
        
        // Aiming Camera
        AimingCamera = virtualCameras.transform.Find("Aiming Camera").GetComponent<CinemachineFreeLook>();
        
        // Interaction Camera
        InCabinetCamera = virtualCameras.transform.Find("InCabinet Camera").GetComponent<CinemachineVirtualCamera>();
        PeekCamera = virtualCameras.transform.Find("Peek Camera").GetComponent<CinemachineVirtualCamera>();
        CubeCamera = virtualCameras.transform.Find("Cube Camera").GetComponent<CinemachineVirtualCamera>();
        
        // Brain Camera Blending Duration Setting
        BrainCamera.m_DefaultBlend.m_Time = _blendingDuration;
        
        Transform playerTransform = transform;

        // Cameras Follow & LookAt Setting
        FollowCamera.Follow = playerTransform;
        FollowCamera.LookAt = playerTransform;

        AimingCamera.Follow = playerTransform;
        AimingCamera.LookAt = playerTransform;
    
        Transform hideableObjectAimTransform = playerTransform.Find("InHideableObjectAim").transform;
        InCabinetCamera.Follow = playerTransform;
        InCabinetCamera.LookAt = hideableObjectAimTransform;

        PeekCamera.Follow = playerTransform;
        PeekCamera.LookAt = _peekPoint;
        _peekCameraComposer = PeekCamera.GetCinemachineComponent<CinemachineComposer>();
        
        // Live 카메라를 Follow로 설정
        FollowCamera.MoveToTopOfPrioritySubqueue();

        // 상호작용 시 설정
        CubeCamera.Follow = null;
        CubeCamera.LookAt = null;
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
        // Camera Orbit Length (Follow Length로 Aiming과 공유)
        float orbitLength = FollowCamera.m_Orbits.Length;
        
        // 앉기 상태에 따라 Offset의 증감 결정
        float offsetDelta = isCrouch ? _heightOffset : -_heightOffset;
        
        // Follow Camera values 
        List<float> followStartHeights = new();
        List<float> followTargetHeights = new();
        List<float> followStartYOffsets = new();
        List<float> followTargetYOffsets = new();
        
        // Aiming Camera Values
        List<float> aimingStartHeights = new();
        List<float> aimingTargetHeights = new();
        List<float> aimingStartYOffsets = new();
        List<float> aimingTargetYOffsets = new();

        for (int i = 0; i < orbitLength; i++)
        {
            // Follow Camera Init
            CinemachineComposer followComposer = FollowCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            followStartHeights.Add(FollowCamera.m_Orbits[i].m_Height);
            followTargetHeights.Add(FollowCamera.m_Orbits[i].m_Height - offsetDelta); 
            
            followStartYOffsets.Add(followComposer.m_TrackedObjectOffset.y);
            followTargetYOffsets.Add(followComposer.m_TrackedObjectOffset.y - offsetDelta);
            
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
                // Follow Camera Lerp Value
                CinemachineComposer followComposer = FollowCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
                FollowCamera.m_Orbits[i].m_Height = Mathf.Lerp(followStartHeights[i], followTargetHeights[i], alpha);
                followComposer.m_TrackedObjectOffset.y = Mathf.Lerp(followStartYOffsets[i], followTargetYOffsets[i], alpha);

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
            // Follow Camera Last Set
            CinemachineComposer followCameraComposer = FollowCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            FollowCamera.m_Orbits[i].m_Height = followTargetHeights[i];
            followCameraComposer.m_TrackedObjectOffset.y = followTargetYOffsets[i];
            
            // Aiming Camera Last Set
            CinemachineComposer aimingComposer = AimingCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            AimingCamera.m_Orbits[i].m_Height = aimingTargetHeights[i];
            aimingComposer.m_TrackedObjectOffset.y = aimingTargetYOffsets[i];
        }

        _changeHeightRoutine = null;
    }

    public void ChangeCameraFromAimingToFollow()
    {
        FollowCamera.MoveToTopOfPrioritySubqueue();

        FollowCamera.m_XAxis.Value = AimingCamera.m_XAxis.Value;
        FollowCamera.m_YAxis.Value = 0.5f;
    }

    public void ChangeCameraFromFollowToAiming()
    {
        AimingCamera.MoveToTopOfPrioritySubqueue();

        AimingCamera.m_XAxis.Value = FollowCamera.m_XAxis.Value;
        AimingCamera.m_YAxis.Value = 0.5f;
    }

    public void ChangeCameraFromFollowToInCabinet()
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

    public void ChangeCameraFromCabinetToFollow()
    {
        FollowCamera.MoveToTopOfPrioritySubqueue();
        FollowCamera.m_YAxis.Value = 0.5f;
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

    public void ChangeCameraToCube(Transform follow, Transform lookAt)
    {
        CubeCamera.Follow = follow;
        CubeCamera.LookAt = lookAt;
        CubeCamera.MoveToTopOfPrioritySubqueue();
    }
    
    public void ChangeCameraFromCubeToFollow()
    {
        FollowCamera.MoveToTopOfPrioritySubqueue();
        CubeCamera.Follow = null;
        CubeCamera.LookAt = null;
    }
}
