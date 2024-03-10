using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : Singleton<DebugManager>
{
    private enum EDebugWindows
    {
        InGameConsole,
        KeyVisualizer,
    }

    [SerializeField] private List<DebugWindowBase> _debugWindows;

    public void OnToggleInGameConsole()
    {
        GameObject window = _debugWindows[(int)EDebugWindows.InGameConsole].gameObject;
        window.SetActive(!window.activeInHierarchy);
    }

    public void OnToggleKeyVisualizer()
    {
        GameObject window = _debugWindows[(int)EDebugWindows.KeyVisualizer].gameObject;
        window.SetActive(!window.activeInHierarchy);
    }
}
