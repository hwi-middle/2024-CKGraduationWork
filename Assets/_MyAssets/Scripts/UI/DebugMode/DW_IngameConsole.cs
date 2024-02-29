using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
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

    [Flags]
    private enum ELogTypeFlags
    {
        Info = 1 << 0,
        Warning = 1 << 1,
        Error = 1 << 2,
        AllFlags = 0b0111,
    }

    private struct LogData
    {
        public string msg;
        public string stackTrace;
        public ELogTypeFlags type;

        public LogData(string msg, string stackTrace, ELogTypeFlags type)
        {
            this.msg = msg;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }

    private CanvasGroup _canvasGroup;
    private readonly List<LogData> _logDataList = new List<LogData>();
    private ELogTypeFlags _currentLogFilter = ELogTypeFlags.AllFlags;

    [SerializeField] private TMP_Text _logText;
    [SerializeField] private Slider _opacitySlider;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private List<Button> _buttons;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null);
        _opacitySlider.onValueChanged.AddListener(OnOpacitySliderValueChanged);

        _buttons[(int)EInGameConsoleButtons.FilterByInfo].onClick.AddListener(() => { ToggleLogTypes(ELogTypeFlags.Info); });
        _buttons[(int)EInGameConsoleButtons.FilterByWarning].onClick.AddListener(() => { ToggleLogTypes(ELogTypeFlags.Warning); });
        _buttons[(int)EInGameConsoleButtons.FilterByError].onClick.AddListener(() => { ToggleLogTypes(ELogTypeFlags.Error); });
        _buttons[(int)EInGameConsoleButtons.Focus].onClick.AddListener(ScrollToBottom);
        _buttons[(int)EInGameConsoleButtons.Clear].onClick.AddListener(OnClickClearButton);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Log for Test ;)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.LogWarning("Warning for Test ;)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.LogError("Error for Test ;)");
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

    private void ScrollToBottom()
    {
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnClickClearButton()
    {
        ClearLogs();
        _logDataList.Clear();
    }

    private void ClearLogs()
    {
        _logText.text = string.Empty;
    }

    private void ToggleLogTypes(ELogTypeFlags type)
    {
        _currentLogFilter ^= type;

        Color infoButtonColor = _currentLogFilter.HasFlag(ELogTypeFlags.Info) ? Color.green : Color.white;
        Color warningButtonColor = _currentLogFilter.HasFlag(ELogTypeFlags.Warning) ? Color.yellow : Color.white;
        Color errorButtonColor = _currentLogFilter.HasFlag(ELogTypeFlags.Error) ? Color.red : Color.white;

        _buttons[(int)EInGameConsoleButtons.FilterByInfo].GetComponent<Image>().color = infoButtonColor;
        _buttons[(int)EInGameConsoleButtons.FilterByWarning].GetComponent<Image>().color = warningButtonColor;
        _buttons[(int)EInGameConsoleButtons.FilterByError].GetComponent<Image>().color = errorButtonColor;

        FilterLogs();
    }

    private void WriteLogMessage(string msg, string stackTrace, LogType type)
    {
        StartCoroutine(WriteLogMessageRoutine(msg, stackTrace, type));
    }

    private void FilterLogs()
    {
        List<LogData> filteredLogs = _logDataList
            .Where(log => (log.type & _currentLogFilter) != 0)
            .ToList();

        ClearLogs();
        foreach (LogData logData in filteredLogs)
        {
            _logText.text += logData.msg;
        }

        ScrollToBottom();
    }

    private IEnumerator WriteLogMessageRoutine(string msg, string stackTrace, LogType type)
    {
        const float TOLERANCE = 0.2f;
        bool isAlreadyScrolledToBottom = _scrollRect.verticalNormalizedPosition <= TOLERANCE;
        var logMsgBuilder = new StringBuilder();
        ELogTypeFlags simplifiedLogType;
        switch (type)
        {
            case LogType.Assert:
            case LogType.Error:
            case LogType.Exception:
                logMsgBuilder.Append("<color=red>[ERROR] ");
                simplifiedLogType = ELogTypeFlags.Error;
                break;
            case LogType.Warning:
                logMsgBuilder.Append("<color=yellow>[WARNING] ");
                simplifiedLogType = ELogTypeFlags.Warning;
                break;
            case LogType.Log:
                logMsgBuilder.Append("<color=white>[INFO] ");
                simplifiedLogType = ELogTypeFlags.Info;
                break;
            default:
                simplifiedLogType = 0;
                Debug.Assert(false);
                break;
        }

        logMsgBuilder.Append($"{msg}</color>\n");
        logMsgBuilder.Append($"<size=12>{stackTrace}</size>\n");

        string logMsg = logMsgBuilder.ToString();
        _logText.text += logMsg;
        _logDataList.Add(new LogData(logMsg, stackTrace, simplifiedLogType));

        yield return new WaitForEndOfFrame(); // OnGUI 실행 대기

        bool isScrollBegin = _scrollRect.verticalScrollbar.size > 0.95f;
        if (isAlreadyScrolledToBottom || isScrollBegin)
        {
            ScrollToBottom();
        }
    }

    private void OnOpacitySliderValueChanged(float value)
    {
        _canvasGroup.alpha = value;
    }
}
