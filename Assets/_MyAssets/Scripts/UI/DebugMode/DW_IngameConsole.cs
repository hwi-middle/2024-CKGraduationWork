using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DW_IngameConsole : DraggableUI
{
    private enum EInGameConsoleButtons
    {
        FilterByInfo,
        FilterByWarning,
        FilterByError,
        Focus,
        Clear,
        Close,
    }
    
    private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _logText;
    [SerializeField] private Slider _opacitySlider;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private List<Button> _buttons;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null);
        _opacitySlider.onValueChanged.AddListener(OnOpacitySliderValueChanged);
        
        _buttons[(int)EInGameConsoleButtons.Focus].onClick.AddListener(OnClickFocusButton);
        _buttons[(int)EInGameConsoleButtons.Clear].onClick.AddListener(OnClickClearButton);
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

    private void OnClickFocusButton()
    {
        ScrollToBottom();
    }

    private void OnClickClearButton()
    {
        _logText.text = string.Empty;
    }

    private void ScrollToBottom()
    {
        _scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    private void WriteLogMessage(string msg, string stackTrace, LogType type)
    {
        StartCoroutine(WriteLogMessageRoutine(msg, stackTrace, type));
    }

    private IEnumerator WriteLogMessageRoutine(string msg, string stackTrace, LogType type)
    {
        const float TOLERANCE = 0.2f;
        bool isScrolledToBottom = _scrollRect.normalizedPosition.y <= TOLERANCE;
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

        yield return new WaitForEndOfFrame();
        if (isScrolledToBottom)
        {
            ScrollToBottom();
        }
    }

    private void OnOpacitySliderValueChanged(float value)
    {
        _canvasGroup.alpha = value;
    }
}
