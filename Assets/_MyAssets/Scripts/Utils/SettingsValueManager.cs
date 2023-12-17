using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum ESettingsValue
{
    Off,
    VeryLow,
    Low,
    Normal,
    High,
    VeryHigh
}

public static class SettingsValueManager
{
    public static float ConvertSettingsValueToFloat(ESettingsValue value)
    {
        // 값이 일정하지 않게 보정될 수 있으므로 switch문으로 구현
        // return (int) value * 0.2f;

        switch (value)
        {
            case ESettingsValue.Off:
                return 0f;
            case ESettingsValue.VeryLow:
                return 0.2f;
            case ESettingsValue.Low:
                return 0.4f;
            case ESettingsValue.Normal:
                return 0.6f;
            case ESettingsValue.High:
                return 0.8f;
            case ESettingsValue.VeryHigh:
                return 1.0f;
            default:
                Debug.Assert(false);
                return -1f;
        }
    }

    public static void ApplyPlayerPrefsValues(AudioSource bgm, AudioSource se, Volume volume)
    {
        // PlayerPrefs 적용
        int defaultValue = (int) ESettingsValue.VeryHigh;
        ESettingsValue currentBgmVolume = (ESettingsValue) PlayerPrefs.GetInt(PlayerPrefsKeyNames.BGM_VOLUME, defaultValue);
        ESettingsValue currentSeVolume = (ESettingsValue) PlayerPrefs.GetInt(PlayerPrefsKeyNames.SE_VOLUME, defaultValue);
        ESettingsValue currentVignetteStrength = (ESettingsValue) PlayerPrefs.GetInt(PlayerPrefsKeyNames.VIGNETTE_STRENGTH, (int) ESettingsValue.Off);

        bgm.volume = ConvertSettingsValueToFloat(currentBgmVolume);
        se.volume = ConvertSettingsValueToFloat(currentSeVolume);

        volume.profile.TryGet(out Vignette vignette);
        Debug.Assert(vignette != null);
        vignette.intensity.value = ConvertSettingsValueToFloat(currentVignetteStrength);
    }
}