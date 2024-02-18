using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneManager : SceneManagerBase
{
    private void Awake()
    {
        GetSettingsValueAndApply();
    }

    protected override void Start()
    {
        base.Start();
        FadeIn(DEFAULT_FADE_DURATION);
    }

    protected override void Update()
    {
        base.Update();
    }

    private void GetSettingsValueAndApply()
    {
        // 해상도
        int width = PlayerPrefs.GetInt(PlayerPrefsKeyNames.RESOLUTION_WIDTH, Screen.width);
        int height = PlayerPrefs.GetInt(PlayerPrefsKeyNames.RESOLUTION_HEIGHT, Screen.height);
        var fullScreenMode = (FullScreenMode)PlayerPrefs.GetInt(PlayerPrefsKeyNames.DISPLAY_MODE, (int)FullScreenMode.ExclusiveFullScreen);
        int refreshRate = PlayerPrefs.GetInt(PlayerPrefsKeyNames.RESOLUTION_REFRESH_RATE, Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value));
        Screen.SetResolution(width, height, fullScreenMode, new RefreshRate()
        {
            numerator = (uint)refreshRate,
            denominator = 1U,
        });
        
        // 텍스처 품질
        QualitySettings.globalTextureMipmapLimit = PlayerPrefs.GetInt(PlayerPrefsKeyNames.TEXTURE_QUALITY, 0);
    }
}
