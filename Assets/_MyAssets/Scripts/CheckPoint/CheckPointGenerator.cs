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
    
    private Vector3 _checkPointPosition;
    
    private void OnEnable()
    {
        _checkPointRootHandler = (CheckPointRootHandler)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _checkPointRootHandler = (CheckPointRootHandler)target;
        
        _checkPointPosition = EditorGUILayout.Vector3Field("CheckPoint Position", _checkPointPosition);
        
        if (GUILayout.Button("Generate CheckPoint"))
        {
            GameObject checkPointPrefab = Instantiate(Resources.Load<GameObject>("CheckPoint/CheckPoint"),
                _checkPointRootHandler.transform);
            checkPointPrefab.transform.position = _checkPointPosition;
            _checkPointRootHandler.CheckPointList.Add(checkPointPrefab);
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
}
