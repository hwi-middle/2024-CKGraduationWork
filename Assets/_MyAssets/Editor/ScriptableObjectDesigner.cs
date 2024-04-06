using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.Serialization;

public enum EScriptableObjectDataType
{
    [InspectorName("int")] Int,
    [InspectorName("float")] Float,
    [InspectorName("string")] String,
    [InspectorName("bool")] Bool,
    [InspectorName("Header")] Header,
}

[System.Serializable]
public struct ScriptableObjectDesignerData
{
    [SerializeField] EScriptableObjectDataType dataType;
    [SerializeField] string dataName;
    [SerializeField] string dataTooltip;
}

public class ScriptableObjectDesigner : EditorWindow
{
    [SerializeField] private string _dataTableName;
    [SerializeField] private List<ScriptableObjectDesignerData> _scriptableObjectData = new List<ScriptableObjectDesignerData>();
    private ReorderableList _reorderableList;

    [SerializeField] private Vector2 _scrollPos = Vector2.zero;
    
    
    private const int PADDING = 10;

    [MenuItem("DMW Tools/ScriptableObject Designer", false, 30)]
    public static void Init()
    {
        ScriptableObjectDesigner window = GetWindow<ScriptableObjectDesigner>();
        window.titleContent = new GUIContent("ScriptableObject Designer");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        var serializedObject = new SerializedObject(this);

        _reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("_scriptableObjectData"), true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Scriptable Object Data"); },
        };

        _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            const int DATA_TYPE_WIDTH = 80;
            if (element.FindPropertyRelative("dataType").enumValueIndex == (int)EScriptableObjectDataType.Header)
            {
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, DATA_TYPE_WIDTH, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("dataType"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + DATA_TYPE_WIDTH + PADDING, rect.y, rect.width - DATA_TYPE_WIDTH - PADDING, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("dataName"), GUIContent.none);
                return;
            }

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, DATA_TYPE_WIDTH, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("dataType"), GUIContent.none);

            float dataNameX = rect.x + DATA_TYPE_WIDTH + PADDING;
            float dataNameWidth = (rect.width - DATA_TYPE_WIDTH - PADDING * 2) * 0.3f;
            EditorGUI.PropertyField(new Rect(dataNameX, rect.y, dataNameWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("dataName"), GUIContent.none);

            float dataTooltipX = dataNameX + dataNameWidth + PADDING;
            float dataTooltipWidth = (rect.width - DATA_TYPE_WIDTH - PADDING * 2) * 0.7f;
            EditorGUI.PropertyField(new Rect(dataTooltipX, rect.y, dataTooltipWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("dataTooltip"), GUIContent.none);
        };
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _reorderableList.DoLayoutList();

        GUILayout.Space(30);
        GUILayout.Button("Create Data Table as ScriptableObject", GUILayout.Height(60));
        EditorGUILayout.EndScrollView();
    }
}
