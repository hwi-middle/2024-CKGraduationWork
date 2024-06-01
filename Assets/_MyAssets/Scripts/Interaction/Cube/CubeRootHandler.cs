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

    [Header("현재 큐브 하이라이트 색상")]
    [SerializeField] private Color _highlightColor;

    private const string AK_CORRECT = "IsCorrect";
    private static readonly int Correct = Animator.StringToHash(AK_CORRECT);

    [SerializeField] private Animator _nextObjectAnimator;
    public bool IsCorrect { get; private set; } = false;

    private Color _originColor;
    private Transform _prevCubeTransform;

    private readonly List<int> _currentCubeRotations = new();
    
    public float CubeRotateDirection { get; set; }
    private float _currentRotationY;

    private int _currentCubeIndex;
    private Transform _currentCubeTransform;
    
    private readonly List<Transform> _cubeList = new();

    private IEnumerator _rotateCubeRoutine;
    
    private int _resetCubeRotationRoutineCount = 0;
    
    public bool IsRotateRoutineRunning => _rotateCubeRoutine != null;
    public bool IsResetRoutineRunning => _resetCubeRotationRoutineCount is not 0;
    
    private bool _isCheckingCubeCorrect;

    private void Start()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            _cubeList.Add(transform.GetChild(i));
        }

        InitCubeRotation();
        InitCubeIndex();
    }

    private void InitCubeRotation()
    {
        Debug.Assert(_cubeCorrectRotations.Count == _cubeInitialRotations.Count, "Cube Rotations Count Mismatch");
        Debug.Assert(_cubeList.Count == _cubeInitialRotations.Count, "CubeList Count Mismatch");

        for (int i = 0; i < _cubeList.Count; i++)
        {
            _cubeList[i].transform.localRotation = Quaternion.Euler(0, _cubeInitialRotations[i] * -ROTATE_DEGREE, 0);
            _currentCubeRotations.Add(_cubeInitialRotations[i]);
        }
    }

    public void InitCubeIndex()
    {
        _currentCubeTransform = _cubeList[0];
        _currentCubeIndex = 0;
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
    }

    public void ResetCube()
    {
        // Todo : 마지막 상관 없이 무조건 필드에 저장 후 종료시키기
        for (int i = 0; i < _cubeList.Count; i++)
        {
            if (_currentCubeRotations[i] != _cubeInitialRotations[i])
            {
                StartCoroutine(ResetCubeRoutine(_cubeList[i], i));
                _resetCubeRotationRoutineCount++;
            }
        }
    }

    private IEnumerator ResetCubeRoutine(Transform cube, int index)
    {
        const float DURATION = 1.0f;
        float t = 0;
        
        Quaternion currentRotation = cube.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, _cubeInitialRotations[index] * -ROTATE_DEGREE, 0);
        
        while (t <= DURATION)
        {
            cube.transform.localRotation = Quaternion.Slerp(currentRotation, targetRotation, t / DURATION);
            yield return null;
            t += Time.deltaTime;
        }
        
        cube.transform.localRotation = targetRotation;
        _currentCubeRotations[index] = _cubeInitialRotations[index];
        _resetCubeRotationRoutineCount--;
    }

    public void HighlightCurrentCube()
    {
        ReturnCubeColorToOrigin();
        
        Material currentCubeMaterial = _currentCubeTransform.GetComponent<Renderer>().material;
        _originColor = currentCubeMaterial.color;
        currentCubeMaterial.color = _highlightColor;
        _prevCubeTransform = _currentCubeTransform;
    }

    public void ReturnCubeColorToOrigin()
    {
        if (_prevCubeTransform is null)
        {
            return;
        }
        
        _prevCubeTransform.GetComponent<Renderer>().material.color = _originColor;
    }

    public void RotateCube()
    {
        if (IsRotateRoutineRunning || _isCheckingCubeCorrect)
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
            ExecuteCorrectCubeSequence();
            CubeInteractionController.Instance.CubeExitWhenCorrect();
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
        for (int i = 0; i < _cubeCorrectRotations.Count; i++)
        {
            if (_cubeCorrectRotations[i] == _currentCubeRotations[i])
            {
                continue;
            }

            IsCorrect = false;
            return false;
        }
        
        IsCorrect = true;
        return true;
    }
    
    private void ExecuteCorrectCubeSequence()
    {
        _nextObjectAnimator.SetBool(Correct, true);
    }

    public void SelectUpperCube()
    {
        int listCount = _cubeList.Count;
        _currentCubeIndex = (_currentCubeIndex - 1 + listCount) % listCount;
        _currentCubeTransform = _cubeList[_currentCubeIndex];
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
        HighlightCurrentCube();
    }

    public void SelectLowerCube()
    {
        int listCount = _cubeList.Count;
        _currentCubeIndex = (_currentCubeIndex + 1) % listCount;
        _currentCubeTransform = _cubeList[_currentCubeIndex];
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
        HighlightCurrentCube();
    }
}
