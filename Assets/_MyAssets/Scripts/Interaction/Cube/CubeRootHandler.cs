using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeRootHandler : MonoBehaviour
{
    [Header("큐브가 돌아가는 시간(초)")]
    [SerializeField] private float _rotateDuration = 2.0f;
    private const float ROTATE_DEGREE = 90.0f;
    
    public float CubeRotateDirection { get; set; }
    private float _currentRotationY;

    private int _currentCubeIndex;
    private Transform _currentCubeTransform;
    
    private readonly List<Transform> _cubeList = new();

    private IEnumerator _rotateCubeRoutine;
    public bool IsRotateRoutineRunning => _rotateCubeRoutine != null;
    
    private List<ESfxAudioClipIndex> _cubeRotateSounds = new();

    private void Awake()
    {
        _cubeRotateSounds.Add(ESfxAudioClipIndex.OB_CubeTurn1);
        _cubeRotateSounds.Add(ESfxAudioClipIndex.OB_CubeTurn2);
        _cubeRotateSounds.Add(ESfxAudioClipIndex.OB_CubeTurn3);
    }

    private void Start()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            _cubeList.Add(transform.GetChild(i));
        }

        _currentCubeTransform = _cubeList[childCount - 1];
        _currentCubeIndex = childCount - 1;
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
        
        AudioPlayManager.Instance.PlayOnceSfxAudio(_cubeRotateSounds[Random.Range(0, _cubeRotateSounds.Count)]);

        bool isSecondSoundPlayed = false;
        float secondSoundTime = _rotateDuration * 0.35f;

        while (t <= _rotateDuration)
        {
            float alpha = t / _rotateDuration;

            float easedAlpha = EasingFunctions.EaseOutBounce(alpha);

            if (t >= secondSoundTime && !isSecondSoundPlayed)
            {
                isSecondSoundPlayed = true;
                AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.OB_CubeTurn4);
            }

            _currentCubeTransform.transform.localRotation =
                Quaternion.Slerp(currentRotation, targetRotation, easedAlpha);
            yield return null;
            t += Time.deltaTime;
        }

        _currentCubeTransform.transform.localRotation = targetRotation;
        _rotateCubeRoutine = null;
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
