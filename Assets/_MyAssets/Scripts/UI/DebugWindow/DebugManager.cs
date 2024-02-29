using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
    private enum EDebugWindows
    {
        InGameConsole,
        KeyVisualizer,
    }

    [SerializeField] private List<DebugWindowBase> _debugWindows;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameObject window = _debugWindows[(int)EDebugWindows.InGameConsole].gameObject;
            window.SetActive(!window.activeInHierarchy);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameObject window = _debugWindows[(int)EDebugWindows.KeyVisualizer].gameObject;
            window.SetActive(!window.activeInHierarchy);
        }
    }
}
