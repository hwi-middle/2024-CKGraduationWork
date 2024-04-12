using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.Serialization;

public enum ESoDataType
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
    public ESoDataType dataType;
    public string name;
    public string tooltipDesc;
    public string headerName;
}

public class ScriptableObjectDesigner : EditorWindow
{
    [SerializeField] private string _dataTableName;
    [SerializeField] private List<ScriptableObjectDesignerData> _scriptableObjectData = new List<ScriptableObjectDesignerData>();
    private ReorderableList _reorderableList;
    private SerializedObject _serializedObject;
    [SerializeField] private Vector2 _scrollPos = Vector2.zero;
    
    private const int PADDING = 10;
    private const int DEFAULT_SPACE = 30;

    [MenuItem("DMW Tools/ScriptableObject Designer", false, 30)]
    public static void Init()
    {
        ScriptableObjectDesigner window = GetWindow<ScriptableObjectDesigner>();
        window.titleContent = new GUIContent("ScriptableObject Designer");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(this);

        _reorderableList = new ReorderableList(_serializedObject, _serializedObject.FindProperty("_scriptableObjectData"), true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "DataTable Design"); },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                const int DATA_TYPE_WIDTH = 80;
                if (element.FindPropertyRelative("dataType").enumValueIndex == (int)ESoDataType.Header)
                {
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, DATA_TYPE_WIDTH, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("dataType"), GUIContent.none);
                    EditorGUI.PropertyField(
                        new Rect(rect.x + DATA_TYPE_WIDTH + PADDING, rect.y, rect.width - DATA_TYPE_WIDTH - PADDING, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("headerName"), GUIContent.none);
                    return;
                }

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, DATA_TYPE_WIDTH, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("dataType"),
                    GUIContent.none);

                float dataNameX = rect.x + DATA_TYPE_WIDTH + PADDING;
                float dataNameWidth = (rect.width - DATA_TYPE_WIDTH - PADDING * 2) * 0.3f;
                EditorGUI.PropertyField(new Rect(dataNameX, rect.y, dataNameWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"),
                    GUIContent.none);

                float tooltipDescX = dataNameX + dataNameWidth + PADDING;
                float tooltipDescWidth = (rect.width - DATA_TYPE_WIDTH - PADDING * 2) * 0.7f;
                EditorGUI.PropertyField(new Rect(tooltipDescX, rect.y, tooltipDescWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("tooltipDesc"), GUIContent.none);
            },
            onAddCallback = (ReorderableList list) =>
            {
                list.serializedProperty.arraySize++;
                var element = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                element.FindPropertyRelative("dataType").enumValueIndex = 0;
                element.FindPropertyRelative("name").stringValue = "데이터 이름을 영문으로 쓰세요";
                element.FindPropertyRelative("tooltipDesc").stringValue = "마우스를 올리면 나오는 설명을 쓰세요";
                element.FindPropertyRelative("headerName").stringValue = "구역을 구분할 헤더의 내용을 쓰세요";
            },
        };
    }

    private void OnGUI()
    {
        _serializedObject.Update();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        GUILayout.Space(DEFAULT_SPACE);
        _dataTableName = EditorGUILayout.TextField("Data Table Name", _dataTableName);
        GUILayout.Space(DEFAULT_SPACE);
        _reorderableList.DoLayoutList();
        GUILayout.Space(DEFAULT_SPACE);
        if (GUILayout.Button("Create Data Table as ScriptableObject", GUILayout.Height(60)))
        {
            SaveAsScriptableObject();
        }
        _serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndScrollView();
    }

    private void SaveAsScriptableObject()
    {
        if (!VerifyData())
        {
            return;
        }

        var dataStringBuilder = new StringBuilder();
        foreach (ScriptableObjectDesignerData data in _scriptableObjectData)
        {
            if (data.dataType == ESoDataType.Header)
            {
                dataStringBuilder.AppendLine($"    [Header(\"{data.headerName}\")]");
            }
            else
            {
                string type = ConvertESoDataTypeToCSharpType(data.dataType);
                dataStringBuilder.AppendLine($"    [Tooltip(\"{data.tooltipDesc}\")] public {type} {data.name};");
            }
        }

        // txt 파일을 읽어옵니다.
        var template = Resources.Load<TextAsset>("template");
        var templateStringBuilder = new StringBuilder(template.text);
        templateStringBuilder.Replace("##FORMATTED_DATA_NAME##", AddSpacesBeforeCapitalLetters(_dataTableName));
        templateStringBuilder.Replace("##DATA_NAME##", _dataTableName);
        templateStringBuilder.Replace("##DATA##", dataStringBuilder.ToString());

        // string path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", _dataTableName, "cs", "Save ScriptableObject");
        // if (path.Length == 0)
        // {
        //     return;
        // }
        string path = $"Assets/_MyAssets/Scripts/Data/{_dataTableName}.cs";

        if (File.Exists(path))
        {
            if (!EditorUtility.DisplayDialog("경고", "이미 같은 이름의 파일이 존재합니다. 덮어쓰시겠습니까?", "네", "아니오"))
            {
                return;
            }
        }
        
        using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (var writer = new StreamWriter(fileStream, Encoding.Unicode))
            {
                writer.WriteLine(templateStringBuilder.ToString());
            }
        }

        AssetDatabase.Refresh();
    }

    string AddSpacesBeforeCapitalLetters(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
            {
                newText.Append(' ');
            }
            newText.Append(text[i]);
        }

        return newText.ToString();
    }

    private bool VerifyData()
    {
        if (string.IsNullOrEmpty(_dataTableName))
        {
            ShowErrorMessage("데이터 테이블의 이름을 입력해주세요.");
            return false;
        }

        if (!_dataTableName.EndsWith("Data"))
        {
            ShowErrorMessage("데이터 테이블의 이름은 'Data'로 끝나야 합니다.");
            return false;
        }

        if (_scriptableObjectData.Count == 0)
        {
            ShowErrorMessage("데이터 테이블에 데이터를 추가해주세요.");
            return false;
        }

        foreach (ScriptableObjectDesignerData data in _scriptableObjectData)
        {
            if (data.dataType == ESoDataType.Header)
            {
                if (string.IsNullOrEmpty(data.headerName))
                {
                    ShowErrorMessage("이름이 존재하지 않는 헤더가 있습니다.");
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(data.name.Trim()))
                {
                    ShowErrorMessage("이름이 존재하지 않는 데이터가 있습니다.");
                    return false;
                }

                var reg = new Regex("^[a-zA-Z_$][\\w$]*$");
                if (!reg.IsMatch(data.name))
                {
                    ShowErrorMessage("이름이 유효하지 않은 데이터가 있습니다. 데이터의 이름은 영문자로 시작해야하며 영문자, 숫자, 언더바(_)만 사용할 수 있습니다.");
                    return false;
                }

                if (string.IsNullOrEmpty(data.tooltipDesc.Trim()))
                {
                    ShowErrorMessage("설명이 입력되지 않은 데이터가 있습니다.");
                    return false;
                }
            }
        }

        return true;
    }

    private void ShowErrorMessage(string msg)
    {
        EditorUtility.DisplayDialog("오류", msg, "확인");
    }

    private string ConvertESoDataTypeToCSharpType(ESoDataType dataType)
    {
        switch (dataType)
        {
            case ESoDataType.Int:
                return "int";
            case ESoDataType.Float:
                return "float";
            case ESoDataType.String:
                return "string";
            case ESoDataType.Bool:
                return "bool";
            case ESoDataType.Header:
            // intentional fall-through
            default:
                Debug.Assert(false);
                return null;
        }
    }
}
