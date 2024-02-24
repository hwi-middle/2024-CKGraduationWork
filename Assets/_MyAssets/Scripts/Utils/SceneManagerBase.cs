using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class SceneManagerBase : Singleton<SceneManagerBase>
{
    private SceneFadeManager _fadeManager;

    public const float DEFAULT_FADE_DURATION = 0.5f;
    public bool IsFading { get; private set; }
    
    protected virtual void Start()
    {
        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
        _fadeManager = SceneFadeManager.Instance;
        _fadeManager.GetComponent<Image>().enabled = true;
    }

    protected virtual void Update()
    {
    }


    public void FadeIn(float duration, float delay = 0f, bool ignoreAudio = false)
    {
        StartCoroutine(FadeInRoutine(duration, delay, ignoreAudio));
    }

    public void FadeOut(float duration, float delay = 0f, bool ignoreAudio = false)
    {
        StartCoroutine(FadeOutRoutine(duration, delay, ignoreAudio));
    }


    private IEnumerator FadeInRoutine(float duration, float delay, bool ignoreAudio)
    {
        IsFading = true;
        if (delay != 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        _fadeManager.FadeIn(duration, ignoreAudio);
        while (_fadeManager.IsPlaying)
        {
            yield return null;
        }

        IsFading = false;
    }

    private IEnumerator FadeOutRoutine(float duration, float delay, bool ignoreAudio)
    {
        IsFading = true;

        if (delay != 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        _fadeManager.FadeOut(duration, ignoreAudio);
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
        FadeOut(DEFAULT_FADE_DURATION, 0f, true);
        while (IsFading)
        {
            yield return null;
        }

        Instantiate(Resources.Load<GameObject>("LoadingCanvas"));
        SceneLoader sceneLoader = SceneLoader.Instance;

        FadeIn(DEFAULT_FADE_DURATION, 0f, true);
        while (IsFading)
        {
            yield return null;
        }

        sceneLoader.LoadScene(sceneName);
    }
}
