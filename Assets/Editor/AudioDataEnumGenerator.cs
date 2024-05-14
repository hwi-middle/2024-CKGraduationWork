using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioClipData))]
public class AudioDataEnumGenerator : Editor
{
    private SerializedProperty _bgmClipList;
    private SerializedProperty _sfxClipList;

    private const string BGM_ENUM_PATH = "Assets/_MyAssets/Data/EBgmAudioClipIndex.cs";
    private const string SFX_ENUM_PATH = "Assets/_MyAssets/Data/ESfxAudioClipIndex.cs";
    
    private const string BGM_ENUM_NAME = "EBgmAudioClipIndex";
    private const string SFX_ENUM_NAME = "ESfxAudioClipIndex";

    private void OnEnable()
    {
        _bgmClipList = serializedObject.FindProperty("bgmClipList");
        _sfxClipList = serializedObject.FindProperty("sfxClipList");
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PrefixLabel("BGM Clip List");
        EditorGUILayout.PropertyField(_bgmClipList, true);
        if (GUILayout.Button("Generate BGM enum List"))
        {
            GenerateBgmEnumList();
        }

        EditorGUILayout.Space(50.0f);
        
        EditorGUILayout.PrefixLabel("SFX Clip List");
        EditorGUILayout.PropertyField(_sfxClipList, true);
        if(GUILayout.Button("Generate SFX enum List"))
        {
            GenerateSfxEnumList();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateBgmEnumList()
    {
        GenerateEnumFile(_bgmClipList, BGM_ENUM_PATH, BGM_ENUM_NAME);
    }

    private void GenerateSfxEnumList()
    {
        GenerateEnumFile(_sfxClipList, SFX_ENUM_PATH, SFX_ENUM_NAME);
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
