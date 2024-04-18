using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : Singleton<CameraController> 
{
    public CinemachineBrain BrainCamera { get; private set; }
    public CinemachineFreeLook FreeLookCamera { get; private set; }
    public CinemachineFreeLook AimingCamera { get; private set; }
    public CinemachineVirtualCamera InCabinetCamera { get; private set; }
    public CinemachineFreeLook PeekCamera { get; private set; }

    public bool IsBlending => BrainCamera.IsBlending;

    private Camera _mainCamera;

    private IEnumerator _changeHeightRoutine;
    public bool IsOnRoutine => _changeHeightRoutine != null;

    [SerializeField] private float _heightOffset = 0.5f;
    [SerializeField] private float _routineDuration = 0.25f;

    public void Awake()
    {
        _mainCamera = Camera.main;
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
        PeekCamera = cameras.transform.Find("Peek Camera").GetComponent<CinemachineFreeLook>();

        // Cameras Follow & LookAt Setting
        Transform playerTransform = transform;
        FreeLookCamera.Follow = playerTransform;
        FreeLookCamera.LookAt = playerTransform;

        AimingCamera.Follow = playerTransform;
        AimingCamera.LookAt = playerTransform;

        Transform hideableObjectAimTransform = playerTransform.Find("InHideableObjectAim").transform;
        InCabinetCamera.Follow = playerTransform;
        InCabinetCamera.LookAt = hideableObjectAimTransform;
        
        // Peek Camera는 동적으로 설정
        
        // Live 카메라를 FreeLook으로 설정
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
        AlignCameraToPlayer();
    }

    public void AlignCameraToPlayer()
    {
        Vector3 playerForward = transform.forward;
        float xAxis = Mathf.Atan2(playerForward.x, playerForward.z) * Mathf.Rad2Deg;
        FreeLookCamera.m_XAxis.Value = xAxis;
        FreeLookCamera.m_YAxis.Value = 0.5f;

        AimingCamera.m_XAxis.Value = xAxis;
        AimingCamera.m_YAxis.Value = 0.5f;
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
        InCabinetCamera.MoveToTopOfPrioritySubqueue();
    }

    public void ChangeCameraFromCabinetToFreeLook()
    {
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
        FreeLookCamera.m_YAxis.Value = 0.5f;
    }

    public void ChangeCameraToPeek(GameObject hideableObject)
    {
        Transform peekPoint = hideableObject.transform.Find("PeekPoint");
        
        PeekCamera.Follow = hideableObject.transform;
        PeekCamera.LookAt = peekPoint;

        PeekCamera.m_XAxis.Value = 0;
        PeekCamera.MoveToTopOfPrioritySubqueue();
    }
}
