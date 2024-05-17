using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRootHandler : MonoBehaviour
{
    [Header("큐브가 돌아가는 시간(초)")]
    [SerializeField] private float _rotateDuration = 2.0f;
    private const float ROTATE_DEGREE = 90.0f;
    
    public float CubeRotateDirection { get; set; }
    private float _currentRotationY;

    private int _currentCubeIndex;
    private Transform _currentCubeTransform;
    
    private List<Transform> _cubeList = new();
    private int _childCount;

    private IEnumerator _rotateCubeRoutine;
    public bool IsRotateRoutineRunning => _rotateCubeRoutine != null;
    
    private void Start()
    {
        _childCount = transform.childCount;

        for (int i = 0; i < _childCount; i++)
        {
            _cubeList.Add(transform.GetChild(i));
        }

        _currentCubeTransform = _cubeList[_childCount - 1];
        _currentCubeIndex = _childCount - 1;
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
    }
    
    public void RotateCube()
    {
        if (_rotateCubeRoutine != null)
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

            float duration = EasingFunctions.EaseInOutBounce(alpha);

            _currentCubeTransform.transform.localRotation =
                Quaternion.Slerp(currentRotation, targetRotation, duration);
            yield return null;
            t += Time.deltaTime;
        }

        _currentCubeTransform.transform.localRotation = targetRotation;
        _rotateCubeRoutine = null;
    }

    public void SelectNextCube()
    {
        _currentCubeIndex = Mathf.Clamp(_currentCubeIndex + 1, 0, _childCount - 1);
        _currentCubeTransform = _cubeList[_currentCubeIndex];
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
    }

    public void SelectPrevCube()
    {
        _currentCubeIndex = Mathf.Clamp(_currentCubeIndex - 1, 0, _childCount - 1);
        _currentCubeTransform = _cubeList[_currentCubeIndex];
        _currentRotationY = _currentCubeTransform.transform.localRotation.eulerAngles.y;
    }
}
