using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialVideoData))]
public class TutorialVideoEnumGenerator : Editor
{
    private const string PATH = "Assets/_MyAssets/Data/ETutorialVideoIndex.cs";
    private const string NAME = "ETutorialVideoIndex";
    
    private SerializedProperty _tutorialVideos;
    private TutorialVideoData _tutorialVideoData;

    private void OnEnable()
    {
        _tutorialVideos = serializedObject.FindProperty("tutorialVideos");
        _tutorialVideoData = (TutorialVideoData)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_tutorialVideos, true);
        if (GUILayout.Button("Generate Video Enum"))
        {
            GenerateEnumFile();    
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateEnumFile()
    {
        const string INDENT = "    ";

        StringBuilder builder = new();
        builder.AppendLine("public enum " + NAME);
        builder.AppendLine("{");
        builder.AppendLine(INDENT + "None,");

        foreach (TutorialVideo data in _tutorialVideoData.tutorialVideos)
        {
            builder.AppendLine(INDENT + data.title + ",");
        }

        builder.Append("}");

        using TextWriter writer = new StreamWriter(PATH, false, Encoding.UTF8);
        writer.Write(builder.ToString());
        writer.Close();
        AssetDatabase.Refresh();

        Debug.Log($"Successfully generated enum file at {PATH}");
    }
}

