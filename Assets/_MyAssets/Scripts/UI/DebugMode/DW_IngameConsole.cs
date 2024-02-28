using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DW_IngameConsole : DraggableUI
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _logText;
    [SerializeField] private Slider _opacitySlider;
    [SerializeField] private Scrollbar _logScrollbar;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null);
        _opacitySlider.onValueChanged.AddListener(OnOpacitySliderValueChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Info");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.LogWarning("Warning");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.LogError("Error");
        }
    }
    
    private void OnEnable()
    {
        Application.logMessageReceived += WriteLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= WriteLogMessage;
    }

    private void WriteLogMessage(string msg, string stackTrace, LogType type)
    {
        string logMsg;
        switch (type)
        {
            case LogType.Assert:
            case LogType.Error:
            case LogType.Exception:
                logMsg = "<color=red>[ERROR] ";
                break;
            case LogType.Warning:
                logMsg = "<color=yellow>[WARNING] ";
                break;
            case LogType.Log:
                logMsg = "<color=white>[INFO] ";
                break;
            default:
                logMsg = string.Empty;
                Debug.Assert(false);
                break;
        }
        
        logMsg += $"{msg}\n";
        logMsg += "</color>";

        _logText.text += logMsg;
    }

    private void OnOpacitySliderValueChanged(float value)
    {
        _canvasGroup.alpha = value;
    }
}
