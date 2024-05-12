using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckPointRootHandler))]
public class CheckPointGenerator : Editor
{
    private CheckPointRootHandler _checkPointRootHandler;
    private int _prevChildCount;
    
    // Generate button 클릭 상태 확인
    private bool _isGenerateButtonClicked;
    
    // 마우스 클릭 이벤트 -> OnSceneGUI으로부터 호출
    private Action _mouseClickAction;
    
    private void OnEnable()
    {
        _checkPointRootHandler = (CheckPointRootHandler)target;
        _mouseClickAction += HandleMouseClickAction;
    }

    private void OnDisable()
    {
        _mouseClickAction -= HandleMouseClickAction;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _checkPointRootHandler = (CheckPointRootHandler)target;
        
        if (GUILayout.Button("Generate CheckPoint"))
        {
            Debug.Log($"Click To Scene");
            _isGenerateButtonClicked = true;
        }

        UpdateCheckPointList();
    }

    private void UpdateCheckPointList()
    {
        int childCount = _checkPointRootHandler.transform.childCount;
        int listCount = _checkPointRootHandler.CheckPointList.Count;

        if (childCount == listCount && _prevChildCount == childCount)
        {
            return;
        }

        _checkPointRootHandler.CheckPointList.Clear();
        for (int i = 0; i < childCount; i++)
        {
            GameObject checkPoint = _checkPointRootHandler.transform.GetChild(i).gameObject;
            checkPoint.transform.name = $"CheckPoint_{i + 1}";
            _checkPointRootHandler.CheckPointList.Add(checkPoint);
        }

        _prevChildCount = childCount;
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && _isGenerateButtonClicked)
        {
            _mouseClickAction?.Invoke();
        }
    }
    
    private void HandleMouseClickAction()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            _isGenerateButtonClicked = false;
            Debug.LogError($"Generate Failed : No Hit Object");
            return;
        }

        const float OFFSET = 0.5f;
        Vector3 hitPosition = hit.point;
        hitPosition.y += OFFSET;
        GenerateCheckPoint(hitPosition);
    }

    private void GenerateCheckPoint(Vector3 position)
    {
        GameObject checkPointPrefab = Instantiate(Resources.Load<GameObject>("CheckPoint/CheckPoint"),
            _checkPointRootHandler.transform);
        checkPointPrefab.transform.position = position;
        _checkPointRootHandler.CheckPointList.Add(checkPointPrefab);

        Debug.Log($"Success to Generate CheckPoint! : Position {position}");
        _isGenerateButtonClicked = false;
    }
}
