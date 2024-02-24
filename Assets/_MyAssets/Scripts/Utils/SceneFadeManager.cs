using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SceneFadeManager : Singleton<SceneFadeManager>
{
    private Image _imgSrc;
    private AudioMixer _audioMixer;
    private bool _isPlaying = false;

    public bool IsPlaying => _isPlaying;

    // Start is called before the first frame update
    void Awake()
    {
        _imgSrc = GetComponent<Image>();
        _audioMixer = Resources.Load<AudioMixer>("AudioMixer/MainMixer");
    }

    public void FadeIn(float t)
    {
        StartCoroutine(FadeInRoutine(t));
    }

    public void FadeOut(float t)
    {
        StartCoroutine(FadeOutRoutine(t));
    }

    private IEnumerator FadeInRoutine(float t)
    {
        Debug.Assert(t > 0);

        if (_isPlaying)
        {
            yield break;
        }
        
        _isPlaying = true;
        Color color = _imgSrc.color;
        _imgSrc.enabled = true;
        color.a = 1f;
        _imgSrc.color = color;

        const float DEFAULT_VOLUME = 0f;
        float targetBgmVolume = PlayerPrefs.GetFloat(PlayerPrefsKeyNames.BGM_VOLUME, DEFAULT_VOLUME);
        float targetSeVolume = PlayerPrefs.GetFloat(PlayerPrefsKeyNames.SE_VOLUME, DEFAULT_VOLUME);

        float time = 0f;
        while (_imgSrc.color.a > 0f)
        {
            color.a = Mathf.Lerp(1f, 0f, time / t);
            _imgSrc.color = color;

            const float MIN_VOLUME = -80f;
            _audioMixer.SetFloat("MasterVolume", Mathf.Lerp(MIN_VOLUME, targetBgmVolume, time / t));

            time += Time.deltaTime;
            yield return null;
        }

        _imgSrc.enabled = false;
        _isPlaying = false;
    }

    private IEnumerator FadeOutRoutine(float t)
    {
        Debug.Assert(t > 0);

        if (_isPlaying)
        {
            yield break;
        }
        
        _isPlaying = true;
        Color color = _imgSrc.color;
        _imgSrc.enabled = true;
        color.a = 0f;
        _imgSrc.color = color;

        _audioMixer.GetFloat("MasterVolume", out float originalVolume);

        float time = 0f;
        while (_imgSrc.color.a < 1f)
        {
            color.a = Mathf.Lerp(0f, 1f, time / t);
            _imgSrc.color = color;
            
            const float MIN_VOLUME = -80f;
            _audioMixer.SetFloat("MasterVolume", Mathf.Lerp(originalVolume, MIN_VOLUME, time / t));

            time += Time.deltaTime;
            yield return null;
        }

        _isPlaying = false;
    }
}
