using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class SceneManagerBase : Singleton<SceneManagerBase>
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private bool _cursorLock;
    private SceneFadeManager _fadeManager;
    private GameObject _settingCanvas;

    private bool _isPaused;

    public const float DEFAULT_FADE_DURATION = 0.5f;
    public bool IsFading { get; private set; }

    protected virtual void OnEnable()
    {
        _inputData.pauseEvent += HandlePauseAction;
    }

    protected void OnDisable()
    {
        _inputData.pauseEvent -= HandlePauseAction;
    }

    protected virtual void Start()
    {
        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
        _fadeManager = SceneFadeManager.Instance;
        _fadeManager.GetComponent<Image>().enabled = true;
        
        _settingCanvas = Instantiate(Resources.Load<GameObject>("SettingCanvas"));
        _settingCanvas.SetActive(false);
        _isPaused = false;
    }

    protected virtual void Update()
    {
    }

    private void HandlePauseAction()
    {
        Debug.Assert(_settingCanvas != null, "_settingCanvas != null");
        
        if (IsFading)
        {
            return;
        }
        
        ToggleSettingCanvas();
    }
    
    
    private void ToggleSettingCanvas()
    {
        _isPaused = !_isPaused;
        
        _settingCanvas.SetActive(_isPaused);
        ToggleCursorVisible();
        Time.timeScale = _isPaused ? 0.0f : 1.0f;
    }

    private void ToggleCursorVisible()
    {
        if (_settingCanvas.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if (!_cursorLock)
        {
            return;
        }
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
