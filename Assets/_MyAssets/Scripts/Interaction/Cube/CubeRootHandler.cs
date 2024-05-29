using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRootHandler : MonoBehaviour
{
    [Header("큐브가 돌아가는 시간(초)")]
    [SerializeField] private float _rotateDuration = 2.0f;
    private const float ROTATE_DEGREE = 90.0f;

    [Header("큐브 초기값")] [Range(0, 3)]
    [SerializeField] private List<int> _cubeInitialRotations;
    
    [Header("큐브 정답")] [Range(0, 3)] 
    [SerializeField] private List<int> _cubeCorrectRotations;

    private List<int> _currentCubeRotations = new();
    
    public float CubeRotateDirection { get; set; }
    private float _currentRotationY;

    private int _currentCubeIndex;
    private Transform _currentCubeTransform;
    
    private readonly List<Transform> _cubeList = new();

    private IEnumerator _rotateCubeRoutine;
    public bool IsRotateRoutineRunning => _rotateCubeRoutine != null;
    private bool _isCheckingCubeCorrect;
    
    private void Start()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            _cubeList.Add(transform.GetChild(i));
        }

        InitCubeRotation();
        
        _currentCubeTransform = _cubeList[childCount - 1];
        _currentCubeIndex = childCount - 1;
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
    }

    private void InitCubeRotation()
    {
        Debug.Assert(_cubeCorrectRotations.Count == _cubeInitialRotations.Count, "Cube Rotations Count Mismatch");
        Debug.Assert(_cubeList.Count == _cubeInitialRotations.Count, "CubeList Count Mismatch");

        int rotationsListIndex = 0;
        for (int i = _cubeList.Count - 1; i >= 0; i--, rotationsListIndex++)
        {
            _cubeList[i].transform.localRotation = Quaternion.Euler(0, _cubeInitialRotations[rotationsListIndex] * -ROTATE_DEGREE, 0);
            _currentCubeRotations.Add(_cubeInitialRotations[i]);
        }
    }

    public void RotateCube()
    {
        if (_rotateCubeRoutine != null || _isCheckingCubeCorrect)
        {
            return;
        }

        _rotateCubeRoutine = RotateCubeRoutine();
        StartCoroutine(_rotateCubeRoutine);
    }

    private IEnumerator RotateCubeRoutine()
    {
        float t = 0;

        Quaternion currentRotation = _currentCubeTransform.transform.localRotation;
        _currentRotationY = currentRotation.eulerAngles.y;
        float rotationY = CubeRotateDirection * ROTATE_DEGREE + _currentRotationY;
        Quaternion targetRotation = Quaternion.Euler(0, rotationY, 0);

        while (t <= _rotateDuration)
        {
            float alpha = t / _rotateDuration;

            float easedAlpha = EasingFunctions.EaseInOutBounce(alpha);

            _currentCubeTransform.transform.localRotation =
                Quaternion.Slerp(currentRotation, targetRotation, easedAlpha);
            yield return null;
            t += Time.deltaTime;
        }

        _currentCubeTransform.transform.localRotation = targetRotation;
        UpdateRotationIndex();
        
        _rotateCubeRoutine = null;
        
        _isCheckingCubeCorrect = true;
        
        if (CheckCubeCorrect())
        {
            CubeInteractionController.Instance.ExecuteCorrectCubeSequence();
        }

        _isCheckingCubeCorrect = false;
    }

    private void UpdateRotationIndex()
    {
        int cubeRotation = _currentCubeRotations[_currentCubeIndex] - (int)CubeRotateDirection;

        switch (cubeRotation)
        {
            case > 3:
                cubeRotation = 0;
                break;
            case < 0:
                cubeRotation = 3;
                break;
        }
        
        _currentCubeRotations[_currentCubeIndex] = cubeRotation;
    }

    private bool CheckCubeCorrect()
    {
        int currentCubeIndex = _currentCubeRotations.Count - 1;
        for (int i = 0; i < _cubeCorrectRotations.Count; i++, currentCubeIndex--)
        {
            if (_cubeCorrectRotations[i] != _currentCubeRotations[currentCubeIndex])
            {
                return false;
            }
        }

        return true;
    }

    public void SelectUpperCube()
    {
        _currentCubeIndex = Mathf.Clamp(_currentCubeIndex + 1, 0, transform.childCount - 1);
        _currentCubeTransform = _cubeList[_currentCubeIndex];
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
    }

    public void SelectLowerCube()
    {
        _currentCubeIndex = Mathf.Clamp(_currentCubeIndex - 1, 0, transform.childCount - 1);
        _currentCubeTransform = _cubeList[_currentCubeIndex];
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
    }
}
