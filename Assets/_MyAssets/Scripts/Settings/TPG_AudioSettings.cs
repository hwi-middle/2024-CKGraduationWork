using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPG_AudioSettings : TabPage
{
    private enum EVolumeSlider
    {
        Master,
        Bgm,
        Se,
    }

    [SerializeField] private List<Slider> _volumeSliders;

    private void Awake()
    {
        for (var sliderType = EVolumeSlider.Master; sliderType <= EVolumeSlider.Se; sliderType++)
        {
            EVolumeSlider type = sliderType; // 람다 캡처용 복사
            _volumeSliders[(int)type].value = PlayerPrefs.GetFloat(GetPlayerPrefsKeyNameBySliderEnum(type), 1.0f);
            _volumeSliders[(int)type].onValueChanged.AddListener((value) => { AdjustVolume(type, value); });
        }
    }

    private void AdjustVolume(EVolumeSlider sliderType, float value)
    {
        _volumeSliders[(int)sliderType].value = value;
        PlayerPrefs.SetFloat(GetPlayerPrefsKeyNameBySliderEnum(sliderType), value);
    }

    private string GetPlayerPrefsKeyNameBySliderEnum(EVolumeSlider slider)
    {
        switch (slider)
        {
            case EVolumeSlider.Master:
                return PlayerPrefsKeyNames.MASTER_VOLUME;
            case EVolumeSlider.Bgm:
                return PlayerPrefsKeyNames.BGM_VOLUME;
            case EVolumeSlider.Se:
                return PlayerPrefsKeyNames.SE_VOLUME;
            default:
                Debug.Assert(false);
                return null;
        }
    }
}
