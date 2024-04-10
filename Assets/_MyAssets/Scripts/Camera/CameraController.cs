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
        StartCoroutine(BetweenFreeLookAndInCabinetRoutine());
        InCabinetCamera.MoveToTopOfPrioritySubqueue();
        BrainCamera.m_DefaultBlend.m_Time = 0.5f;
    }

    public void ChangeCameraFromPeekToInCabinet()
    {
        InCabinetCamera.MoveToTopOfPrioritySubqueue();
    }

    public void ChangeCameraFromCabinetToFreeLook()
    {
        StartCoroutine(BetweenFreeLookAndInCabinetRoutine());
        FreeLookCamera.MoveToTopOfPrioritySubqueue();
        FreeLookCamera.m_YAxis.Value = 0.5f;
    }

    private IEnumerator BetweenFreeLookAndInCabinetRoutine()
    {
        BrainCamera.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        yield return new WaitForEndOfFrame();
        BrainCamera.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
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
