using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TPG_Graphics : TabPage
{
    private readonly struct ResolutionData
    {
        public int Width { get; }
        public int Height { get; }
        public int RefreshRate { get; }

        public ResolutionData(int width, int height, RefreshRate refreshRate)
        {
            Width = width;
            Height = height;
            RefreshRate = Mathf.RoundToInt((float)refreshRate.value);
        }

        public ResolutionData(int width, int height, int refreshRate)
        {
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
        }

        public override string ToString()
        {
            string resData = $"{Width} x {Height} ({RefreshRate}Hz)";
            if (IsFitToClientRatio(this))
            {
                resData += " *";
            }

            return resData;
        }

        private bool IsFitToClientRatio(ResolutionData resData)
        {
            return Mathf.Approximately(resData.Width / (float)resData.Height, Screen.width / (float)Screen.height);
        }
    }

    private enum EDisplayModeIndex
    {
        FullScreen,
        FullScreenWindow,
        Windowed,
    }

    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    private readonly List<ResolutionData> _resolutionOptions = new List<ResolutionData>();
    [SerializeField] private TMP_Dropdown _displayModeDropdown;
    private readonly List<string> _displayModeOptions = new List<string>() { "전체화면", "테두리 없는 창모드", "창모드" };

    private void Start()
    {
        // 해상도
        Resolution[] resolutions = Screen.resolutions;
        Array.Reverse(resolutions);

        int currentResolutionIndex = -1;
        for (int index = 0; index < resolutions.Length; index++)
        {
            Resolution resolution = resolutions[index];
            _resolutionOptions.Add(new ResolutionData(resolution.width, resolution.height, resolution.refreshRateRatio));

            if (Screen.width == resolution.width
                && Screen.height == resolution.height
                && Mathf.Approximately((float)Screen.currentResolution.refreshRateRatio.value, (float)resolution.refreshRateRatio.value))
            {
                currentResolutionIndex = index;
            }
        }

        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(_resolutionOptions.ConvertAll(data => data.ToString()));
        _resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        Debug.Assert(currentResolutionIndex != -1);
        _resolutionDropdown.value = currentResolutionIndex;

        // 디스플레이 모드
        _displayModeDropdown.ClearOptions();
        _displayModeDropdown.AddOptions(_displayModeOptions);
        _displayModeDropdown.onValueChanged.AddListener(index => { ChangeDisplayMode((EDisplayModeIndex)index); });
    }

    private void ChangeResolution(int index)
    {
        Debug.Assert(index >= 0 && index < _resolutionOptions.Count);
        ResolutionData resolutionData = _resolutionOptions[index];
        Screen.SetResolution(resolutionData.Width, resolutionData.Height, Screen.fullScreenMode, new RefreshRate()
        {
            numerator = (uint)resolutionData.RefreshRate,
            denominator = 1U,
        });
        PlayerPrefs.SetInt(PlayerPrefsKeyNames.RESOLUTION_WIDTH, resolutionData.Width);
        PlayerPrefs.SetInt(PlayerPrefsKeyNames.RESOLUTION_HEIGHT, resolutionData.Height);
        PlayerPrefs.SetInt(PlayerPrefsKeyNames.RESOLUTION_REFRESH_RATE, resolutionData.RefreshRate);
    }

    private void ChangeDisplayMode(EDisplayModeIndex displayModeIndex)
    {
        switch (displayModeIndex)
        {
            case EDisplayModeIndex.FullScreen:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case EDisplayModeIndex.FullScreenWindow:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case EDisplayModeIndex.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            default:
                Debug.Assert(false);
                return;
        }

        PlayerPrefs.SetInt(PlayerPrefsKeyNames.DISPLAY_MODE, (int)Screen.fullScreenMode);
    }
}
