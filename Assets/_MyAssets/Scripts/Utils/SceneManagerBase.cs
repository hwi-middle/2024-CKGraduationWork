using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class SceneManagerBase : Singleton<SceneManagerBase>
{
    private SceneFadeManager _fadeManager;
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _seAudioSource;
    
    public bool IsFading { get; private set; }

    // protected SceneFadeManager FadeManager => _fadeManager;

    protected virtual void Start()
    {
        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
        _fadeManager = SceneFadeManager.Instance;
        _fadeManager.GetComponent<Image>().enabled = true;
    }

    protected virtual void Update()
    {
    }


    public void FadeIn(float duration, float delay = 0f)
    {
        StartCoroutine(FadeInRoutine(duration, delay, _bgmAudioSource, _seAudioSource));
    }

    public void FadeOut(float duration, float delay = 0f)
    {
        StartCoroutine(FadeOutRoutine(duration, delay, _bgmAudioSource, _seAudioSource));
    }


    private IEnumerator FadeInRoutine(float duration, float delay, AudioSource bgmOrNull, AudioSource seOrNull)
    {
        IsFading = true;
        if (delay != 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        _fadeManager.FadeIn(duration, bgmOrNull, seOrNull);
        while (_fadeManager.IsPlaying)
        {
            yield return null;
        }

        IsFading = false;
    }

    private IEnumerator FadeOutRoutine(float duration, float delay, AudioSource bgmOrNull, AudioSource seOrNull)
    {
        IsFading = true;

        if (delay != 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        _fadeManager.FadeOut(duration, bgmOrNull, seOrNull);
        while (_fadeManager.IsPlaying)
        {
            yield return null;
        }

        IsFading = false;
    }

    protected void LoadSceneWithLoadingUI(string sceneName)
    {
        StartCoroutine(LoadSceneWithLoadingUiRoutine(sceneName));
    }

    private IEnumerator LoadSceneWithLoadingUiRoutine(string sceneName)
    {
        FadeOut(0.5f);
        while (IsFading)
        {
            yield return null;
        }

        Instantiate(Resources.Load<GameObject>("LoadingCanvas"));
        SceneLoader sceneLoader = SceneLoader.Instance;
        
        FadeIn(0.5f);
        while (IsFading)
        {
            yield return null;
        }
        sceneLoader.LoadScene(sceneName);
    }
}
