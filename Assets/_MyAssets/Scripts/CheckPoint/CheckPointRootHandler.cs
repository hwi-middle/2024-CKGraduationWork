using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CheckPointPlayerPrefsNames
{
    public const string LAST_CHECK_POINT = "LastCheckPoint";
    public const string LAST_SCENE_NAME = "LastSceneName";
}

public class CheckPointRootHandler : Singleton<CheckPointRootHandler>
{
    [SerializeField] private List<GameObject> _checkPointList = new();
    public List<GameObject> CheckPointList => _checkPointList;
    
    private readonly string _lastCheckPoint = CheckPointPlayerPrefsNames.LAST_CHECK_POINT;
    private readonly string _lastSceneName = CheckPointPlayerPrefsNames.LAST_SCENE_NAME;
    private int _currentIndex;
    
    private bool IsSameScene => PlayerPrefs.GetString(_lastSceneName).Equals(SceneManager.GetActiveScene().name);

    public bool HasSavedCheckPointData =>
        !PlayerPrefs.GetString(CheckPointPlayerPrefsNames.LAST_SCENE_NAME).Equals(SceneNames.MAIN_MENU);
    
    public Vector3 LastCheckPoint
    {
        get
        {
            if (_currentIndex == -1)
            {
                return Vector3.zero;
            }

            return IsSameScene ? transform.GetChild(_currentIndex).position : Vector3.zero;
        }
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        // Key가 없다면 가장 처음 게임을 킨 상태 -> 모두 초기값으로 설정 후 return
        if (!PlayerPrefs.HasKey(_lastSceneName))
        {
            EnterNewScene();
            return;
        }

        // Key가 있지만 리스폰 혹은 실행 시 Value와 같지 않은 Scene 이라면 Key에 대한 Value를 현재 Scene으로 설정 후 Return
        if (!IsSameScene)
        {
            EnterNewScene();
            return;
        }

        // 체크포인트에 대한 Key가 있으면서 Value가 -1 이 아니라면
        // 해당하는 Scene에서 특정 체크포인트에 닿은적이 있음 -> 해당하는 곳으로 이동
        if (PlayerPrefs.HasKey(_lastCheckPoint) && PlayerPrefs.GetInt(_lastCheckPoint) != -1)
        {
            _currentIndex = PlayerPrefs.GetInt(_lastCheckPoint);
            transform.GetChild(_currentIndex).GetComponent<Collider>().enabled = false;
            return;
        }

        // Key가 없거나 Value가 -1 이라면 
        // 체크포인트에 대한 Key와 Value를 Set 해주고 Index를 -1로 설정하고 시작
        PlayerPrefs.SetInt(_lastCheckPoint, -1);
        _currentIndex = -1;
    }

    private void EnterNewScene()
    {
        // Scene에 대한 Key와 Value를 현재 Scene 명으로
        // CheckPoint에 대한 Key와 Value를 -1로 -> 첫 체크포인트의 Index는 0
        PlayerPrefs.SetString(_lastSceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt(_lastCheckPoint, -1);
        _currentIndex = -1;
    }

    public void SetNewCheckPoint()
    {
        PlayerPrefs.SetInt(_lastCheckPoint, ++_currentIndex);
    }
}
