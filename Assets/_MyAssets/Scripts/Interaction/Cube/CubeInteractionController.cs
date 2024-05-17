using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInteractionController : Singleton<CubeInteractionController>
{
    [SerializeField] private PlayerInputData _inputData;

    private CubeRootHandler _currentCubeRoot;
    public bool HasInteractionCube => _currentCubeRoot != null;

    private bool _isRotatingCube;
    
    private void OnEnable()
    {
        _inputData.selectEvent += HandleSelectEvent;
        _inputData.rotateEvent += HandleRotateEvent;
    }

    private void OnDisable()
    {
        _inputData.selectEvent -= HandleSelectEvent;
        _inputData.rotateEvent -= HandleRotateEvent;
    }

    private void HandleSelectEvent(float value)
    {
        if(_currentCubeRoot.IsRotateRoutineRunning)
        {
            return;
        }
        
        switch (value)
        {
            case 0:
                return;
            // W key Input
            case 1:
                _currentCubeRoot.SelectNextCube();
                return;
            
            // S key Input
            case -1:
                _currentCubeRoot.SelectPrevCube();
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    private void HandleRotateEvent(float value)
    {
        if (_currentCubeRoot.IsRotateRoutineRunning)
        {
            return;
        }

        _currentCubeRoot.CubeRotateDirection = value;
        _currentCubeRoot.RotateCube();
    }

    public void SetInteractionCube(GameObject cubeRoot)
    {
        Debug.Assert(cubeRoot.transform.childCount != 0, "Invalid Cube Object");
        _currentCubeRoot = cubeRoot.GetComponent<CubeRootHandler>();
        PlayerInputData.ChangeInputMap(PlayerInputData.EInputMap.CubeAction);
    }
}
