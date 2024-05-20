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
    private AudioClipData _audioClipData;
        
    private SerializedProperty _maxAudioObjectCount;
    private SerializedProperty _bgmClipList;
    private SerializedProperty _sfxClipList;

    private const string BGM_ENUM_PATH = "Assets/_MyAssets/Data/EBgmAudioClipIndex.cs";
    private const string SFX_ENUM_PATH = "Assets/_MyAssets/Data/ESfxAudioClipIndex.cs";
    
    private const string BGM_ENUM_NAME = "EBgmAudioClipIndex";
    private const string SFX_ENUM_NAME = "ESfxAudioClipIndex";

    private void OnEnable()
    {
        _audioClipData = (AudioClipData)target;
        _maxAudioObjectCount = serializedObject.FindProperty("audioObjectMaxCapacity");
        _bgmClipList = serializedObject.FindProperty("bgmClipList");
        _sfxClipList = serializedObject.FindProperty("sfxClipList");
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 300;
        EditorGUILayout.PrefixLabel("Max Audio Object Count (BGM Excluded)");
        int sliderValue = EditorGUILayout.IntSlider(_maxAudioObjectCount.intValue, 10, 100);
        _maxAudioObjectCount.intValue = sliderValue;
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.PrefixLabel("배경음악 목록");
        EditorGUILayout.PropertyField(_bgmClipList, true);
        if (GUILayout.Button("Generate BGM enum List"))
        {
            GenerateBgmEnumList();
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PrefixLabel("효과음 목록");
        EditorGUILayout.PropertyField(_sfxClipList, true);
        if(GUILayout.Button("Generate SFX enum List"))
        {
            GenerateSfxEnumList();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateBgmEnumList()
    {
        GenerateEnumFile(EAudioType.Bgm, BGM_ENUM_PATH, BGM_ENUM_NAME);
    }

    private void GenerateSfxEnumList()
    {
        GenerateEnumFile(EAudioType.Sfx, SFX_ENUM_PATH, SFX_ENUM_NAME);
    }

    private void GenerateEnumFile(EAudioType type, string path, string enumName)
    {
        StringBuilder builder = new();
        const string INDENT = "    ";

        builder.AppendLine($"public enum {enumName}");
        builder.AppendLine("{");
        builder.AppendLine(INDENT + "None,");

        List<AudioClipInfo> clipInfoList = null;
        
        switch (type)
        {
            case EAudioType.Bgm:
                clipInfoList = _audioClipData.bgmClipList;
                break;
            case EAudioType.Sfx:
                clipInfoList = _audioClipData.sfxClipList;
                break;
            default:
                Debug.Assert(false);
                break;
        }
        
        Debug.Assert(clipInfoList != null, "clipInfoList != null");

        foreach (AudioClipInfo clipInfo in clipInfoList)
        {
            builder.AppendLine(INDENT + clipInfo.clipName + ",");
        }
        builder.Append("}");

        using TextWriter writer = new StreamWriter(path, false, Encoding.UTF8);
        writer.Write(builder.ToString());
        writer.Close();
        AssetDatabase.Refresh();
    }
}
