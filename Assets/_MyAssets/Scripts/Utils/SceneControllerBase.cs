using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class SceneControllerBase : MonoBehaviour
{
    private SceneFadeManager _fadeManager;
    protected bool IsFading { get; private set; }

    // protected SceneFadeManager FadeManager => _fadeManager;

    protected virtual void Start()
    {
        _fadeManager = SceneFadeManager.Instance;
        _fadeManager.GetComponent<Image>().enabled = true;
    }

    protected virtual void Update()
    {
    }


    protected void FadeIn(float duration, float delay = 0f, AudioSource bgmOrNull = null, AudioSource seOrNull = null)
    {
        StartCoroutine(FadeInRoutine(duration, delay, bgmOrNull, seOrNull));
    }

    protected void FadeOut(float duration, float delay = 0f, AudioSource bgmOrNull = null, AudioSource seOrNull = null)
    {
        StartCoroutine(FadeOutRoutine(duration, delay, bgmOrNull, seOrNull));
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
}
