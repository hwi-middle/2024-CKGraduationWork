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
        WriteEnumFile("ETutorialVideoIndex", _tutorialVideoData.tutorialVideos);
    }
    
    private void WriteEnumFile(string enumName, List<TutorialVideo> enumValues)
    {
        if (File.Exists(PATH))
        {
            File.Delete(PATH);
        }

        const string TAB = "    ";
        using (StreamWriter writer = new(PATH))
        {
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");
            writer.WriteLine(TAB + "None,");
            
            foreach (TutorialVideo data in enumValues)
            {
                writer.WriteLine(TAB + data.title + ",");
            }
            
            writer.Write("}");
        }
        
        AssetDatabase.Refresh();
        
        Debug.Log($"Successfully generated enum file at {PATH}");
    }
}

