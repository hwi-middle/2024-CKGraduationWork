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
        for (int i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] != "-BuildOnCloud")
            {
                continue;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, new string[] { "__BUILD_ON_CLOUD" });

            if (i + 1 < arguments.Length)
            {
                PlayerPrefs.SetString(PlayerPrefsKeyName.GIT_COMMIT_HASH, arguments[i + 1]);
                PlayerPrefs.SetString(PlayerPrefsKeyName.GIT_COMMIT_HASH_SHORT, arguments[i + 1].Substring(0, 7));
            }

            break;
        }
    }
}
