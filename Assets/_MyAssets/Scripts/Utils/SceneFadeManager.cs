using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeManager : Singleton<SceneFadeManager>
{
    private Image _imgSrc;
    private bool _isPlaying = false;

    public bool IsPlaying => _isPlaying;
    private const float DEFAULT_FADE_DURATION = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        _imgSrc = GetComponent<Image>();
    }

    public void FadeIn(float t = DEFAULT_FADE_DURATION, AudioSource bgmOrNull = null, AudioSource seOrNull = null)
    {
        StartCoroutine(FadeInRoutine(t, bgmOrNull, seOrNull));
    }

    public void FadeOut(float t = DEFAULT_FADE_DURATION, AudioSource bgmOrNull = null, AudioSource seOrNull = null)
    {
        StartCoroutine(FadeOutRoutine(t, bgmOrNull, seOrNull));
    }

    private IEnumerator FadeInRoutine(float t, AudioSource bgm, AudioSource se)
    {
        Debug.Assert(t > 0);

        _imgSrc.enabled = true;
        if (_isPlaying) yield break;

        _isPlaying = true;
        Color color = _imgSrc.color;

        int defaultValue = (int)ESettingsValue.VeryHigh;
        float targetBgmVolume = SettingsValueManager.ConvertSettingsValueToFloat((ESettingsValue)PlayerPrefs.GetInt(PlayerPrefsKeyNames.BGM_VOLUME, defaultValue));
        float targetSeVolume = SettingsValueManager.ConvertSettingsValueToFloat((ESettingsValue)PlayerPrefs.GetInt(PlayerPrefsKeyNames.SE_VOLUME, defaultValue));

        float time = 0f;
        while (_imgSrc.color.a > 0f)
        {
            color.a = Mathf.Lerp(1f, 0f, time / t);
            _imgSrc.color = color;
            if (bgm != null)
            {
                bgm.volume = Mathf.Lerp(0f, targetBgmVolume, time / t);
            }

            if (se != null)
            {
                se.volume = Mathf.Lerp(0f, targetSeVolume, time / t);
            }

            time += Time.deltaTime;
            yield return null;
        }

        _imgSrc.enabled = false;
        _isPlaying = false;
    }

    private IEnumerator FadeOutRoutine(float t, AudioSource bgm, AudioSource se)
    {
        Debug.Assert(t > 0);

        _imgSrc.enabled = true;
        if (_isPlaying) yield break;

        _isPlaying = true;
        Color color = _imgSrc.color;

        float originalBgmVolume = 0f;
        if (bgm != null)
        {
            originalBgmVolume = bgm.volume;
        }

        float originalSeVolume = 0f;
        if (se != null)
        {
            originalSeVolume = se.volume;
        }

        float time = 0f;
        while (_imgSrc.color.a < 1f)
        {
            color.a = Mathf.Lerp(0f, 1f, time / t);
            _imgSrc.color = color;
            if (bgm != null)
            {
                bgm.volume = Mathf.Lerp(originalBgmVolume, 0f, time / t);
            }

            if (se != null)
            {
                se.volume = Mathf.Lerp(originalSeVolume, 0f, time / t);
            }

            time += Time.deltaTime;
            yield return null;
        }

        _isPlaying = false;
    }
}
