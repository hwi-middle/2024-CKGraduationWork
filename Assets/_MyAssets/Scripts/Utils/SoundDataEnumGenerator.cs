using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundClipData))]
public class SoundDataEnumGenerator : Editor
{
    private SerializedProperty _bgmClipList;
    private SerializedProperty _sfxClipList;

    private const string BGM_ENUM_PATH = "Assets/_MyAssets/Data/EBgmAudioClipIndex.cs";
    private const string SFX_ENUM_PATH = "Assets/_MyAssets/Data/ESfxAudioClipIndex.cs";

    private void OnEnable()
    {
        _bgmClipList = serializedObject.FindProperty("bgmClipList");
        _sfxClipList = serializedObject.FindProperty("sfxClipList");
    }
    
    public override void OnInspectorGUI()
    {
        GUIStyle style = EditorStyles.helpBox;
        GUILayout.BeginVertical(style);
        
        EditorGUILayout.PropertyField(_bgmClipList, true);
        if (GUILayout.Button("Generate BGM enum List"))
        {
            GenerateBgmEnumList();
        }

        EditorGUILayout.PropertyField(_sfxClipList, true);
        if(GUILayout.Button("Generate SFX enum List"))
        {
            GenerateSfxEnumList();
        }
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateBgmEnumList()
    {
        GenerateEnumFile(_bgmClipList, BGM_ENUM_PATH, "EBgmAudioClipIndex");
    }

    private void GenerateSfxEnumList()
    {
        GenerateEnumFile(_sfxClipList, SFX_ENUM_PATH, "ESfxAudioClipIndex");
    }

    private void GenerateEnumFile(SerializedProperty clipList, string path, string enumName)
    {
        StringBuilder builder = new();
        string startString = $"public enum {enumName}\n{{\n    ";
        const string NONE = "None";
        const string END = ",\n    ";

        builder.Append(startString);
        builder.Append(NONE);
        builder.Append(END);
        
        for (int i = 0; i < clipList.arraySize; i++)
        {
            SerializedProperty clip = clipList.GetArrayElementAtIndex(i);

            if (clip.objectReferenceValue == null)
            {
                continue;
            }
            
            string clipName = clip.objectReferenceValue.name;
            builder.Append(clipName);

            if (i == clipList.arraySize - 1)
            {
                break;
            }
            
            builder.Append(END);
        }
        
        builder.Append("\n}");

        using TextWriter writer = new StreamWriter(path, false, Encoding.Unicode);
        writer.Write(builder.ToString());
        writer.Close();
    }

}
