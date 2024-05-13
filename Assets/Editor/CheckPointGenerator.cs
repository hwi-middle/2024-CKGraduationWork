using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckPointRootHandler))]
public class CheckPointGenerator : Editor
{
    private GameObject _checkPointPrefab;
    private CheckPointRootHandler _checkPointRootHandler;
    private int _prevChildCount;
    
    // Generate button 클릭 상태 확인
    private bool _isGenerateButtonClicked;
    
    private Vector3 _mouseWorldPosition;
    private Collider[] _overlappedCheckPoint = new Collider[1];
    
    private void OnEnable()
    {
        _checkPointRootHandler = (CheckPointRootHandler)target;
        _checkPointPrefab = Resources.Load<GameObject>("CheckPoint/CheckPoint");
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
            HandleMouseClickAction();
        }
    }
    
    private void HandleMouseClickAction()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        if (!IsAnyObjectHit(mousePosition) || !IsOverlappedObjectExist())
        {
            return;
        }

        GenerateCheckPoint();
    }

    private bool IsAnyObjectHit(Vector3 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            _isGenerateButtonClicked = false;
            Debug.LogError($"Generate Failed : No Hit Object");
            return false;
        }

        const float OFFSET = 0.5f;
        _mouseWorldPosition = hit.point;
        _mouseWorldPosition.y += OFFSET;
        return true;
    }

    private bool IsOverlappedObjectExist()
    {
        const float RADIUS = 1.0f;
        int layerMask = LayerMask.GetMask("CheckPoint");

        int count = Physics.OverlapSphereNonAlloc(_mouseWorldPosition, RADIUS, _overlappedCheckPoint, layerMask,
            QueryTriggerInteraction.Collide);

        if (count is 0)
        {
            return true;
        }

        _isGenerateButtonClicked = false;
        Debug.LogError($"Generate Failed : Already Exist CheckPoint Nearby");
        return false;
    }

    private void GenerateCheckPoint()
    {
        GameObject checkPoint = Instantiate(_checkPointPrefab, _checkPointRootHandler.transform);
        checkPoint.transform.position = _mouseWorldPosition;
        _checkPointRootHandler.CheckPointList.Add(checkPoint);

        Debug.Log($"Success to Generate CheckPoint! : Position {_mouseWorldPosition}");
        _isGenerateButtonClicked = false;
    }
}
