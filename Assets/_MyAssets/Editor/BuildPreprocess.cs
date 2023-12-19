using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Player;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get; } = 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string[] arguments = System.Environment.GetCommandLineArgs();
        foreach (string arg in arguments)
        {
            if (arg != "-BuildOnCloud")
            {
                continue;
            }
            
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, new string[] { "__BUILD_ON_CLOUD"});
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.WebGL, new string[] { "__BUILD_ON_CLOUD"});
            break;
        }
    }
}
