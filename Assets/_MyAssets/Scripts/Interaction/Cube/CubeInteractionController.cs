using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECubeSelectDirection
{
    Down = -1,
    None = 0,
    Up = 1
}

public class CubeInteractionController : Singleton<CubeInteractionController>
{
    [SerializeField] private PlayerInputData _inputData;

    private CubeRootHandler _currentCubeRoot;

    private bool _isRotatingCube;
    
    private void OnEnable()
    {
        _inputData.selectEvent += HandleSelectEvent;
        _inputData.rotateEvent += HandleRotateEvent;
        _inputData.cubeExitEvent += HandleCubeExitEvent;
    }

    private void OnDisable()
    {
        _inputData.selectEvent -= HandleSelectEvent;
        _inputData.rotateEvent -= HandleRotateEvent;
        _inputData.cubeExitEvent -= HandleCubeExitEvent;
    }

    private void HandleSelectEvent(float value)
    {
        ECubeSelectDirection direction = (ECubeSelectDirection)value;
        if(_currentCubeRoot.IsRotateRoutineRunning || _currentCubeRoot.IsResetRoutineRunning)
        {
            return;
        }

        switch (direction)
        {
            case ECubeSelectDirection.None:
                return;
            // W key Input
            case ECubeSelectDirection.Up:
                _currentCubeRoot.SelectUpperCube();
                return;

            // S key Input
            case ECubeSelectDirection.Down:
                _currentCubeRoot.SelectLowerCube();
                return;
            default:
                Debug.Assert(false);
                break;
        }
    }

    private void HandleRotateEvent(float value)
    {
        if (_currentCubeRoot.IsRotateRoutineRunning || _currentCubeRoot.IsResetRoutineRunning)
        {
            return;
        }

        // Value is 1 -> Right, Value is -1 -> Left
        _currentCubeRoot.CubeRotateDirection = value;
        _currentCubeRoot.RotateCube();
    }

    public void SetCurrentCube(Transform cubeFollowForCamera, GameObject cubeRoot)
    {
        Debug.Assert(cubeRoot.transform.childCount != 0, "Invalid Cube Object");
        _currentCubeRoot = cubeRoot.GetComponent<CubeRootHandler>();
        PlayerInputData.ChangeInputMap(PlayerInputData.EInputMap.CubeAction);
        CameraController.Instance.ChangeCameraToCube(cubeFollowForCamera, cubeRoot.transform);
        _currentCubeRoot.InitCubeIndex();
        _currentCubeRoot.HighlightCurrentCube();
    }

    public void ExecuteCorrectCubeSequence()
    {
        HandleCubeExitEvent();
        
        // TODO : 큐브 정답 이후 처리
    }

    private void HandleCubeExitEvent()
    {
        if(_currentCubeRoot.IsRotateRoutineRunning)
        {
            return;
        }

        if (!_currentCubeRoot.IsCorrect && !_currentCubeRoot.IsResetRoutineRunning)
        {
            _currentCubeRoot.ResetCube();
        }
        
        _currentCubeRoot.ReturnCubeColorToOrigin();
        _currentCubeRoot = null;
        PlayerInputData.ChangeInputMap(PlayerInputData.EInputMap.PlayerAction);
        CameraController.Instance.ChangeCameraFromCubeToFreeLook();
    }
}
