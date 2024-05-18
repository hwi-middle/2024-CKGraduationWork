using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

[RequireComponent(typeof(PopupHandler))]
[RequireComponent(typeof(CursorLockModeUpdater))]
public abstract class SceneManagerBase : Singleton<SceneManagerBase>
{
    [SerializeField] private PlayerInputData _inputData;
    
    [SerializeField] private bool _isDebugMode;
    [SerializeField] private bool _isNeedCursorLock;

    public bool IsNeedCursorLock => _isNeedCursorLock;
    public bool IsDebugMode => _isDebugMode;
    
    [SerializeField] GameObject _audioManager;
    private SceneFadeManager _fadeManager;
    
    private GameObject _settingsCanvas;
    private GameObject _pauseCanvas;

    public bool IsSettingsCanvasActive => _settingsCanvas.activeSelf;
    public bool IsPauseCanvasActive => _pauseCanvas.activeSelf;

    private bool _isPaused;

    public const float DEFAULT_FADE_DURATION = 0.5f;
    public bool IsFading { get; private set; }

    protected Player _player;
    private IEnumerator _playerDeadSequence;

    protected virtual void Awake()
    {
#if !UNITY_EDITOR
        _isDebugMode = false;
#endif
        
        Debug.Assert(_audioManager != null, "Require Audio Manager");
        GetSettingsValueAndApply();
    }

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

        _settingsCanvas = Instantiate(Resources.Load<GameObject>("SettingsCanvas"));
        _settingsCanvas.SetActive(false);

        _pauseCanvas = Instantiate(Resources.Load<GameObject>("PauseCanvas"));
        _pauseCanvas.SetActive(false);

        Instantiate(Resources.Load<GameObject>("EventSystem"));
        
        _isPaused = false;
    }

    protected virtual void Update()
    {
    }

    protected void ExecuteDeadSequence()
    {
        if (!_player.IsPlayerDead() || _playerDeadSequence != null)
        {
            return;
        }

        _playerDeadSequence = PlayerDeadRoutine();
        StartCoroutine(_playerDeadSequence);
    }

    private IEnumerator PlayerDeadRoutine()
    {
        PlayerMove.Instance.SetDeadState();
        const float DEAD_ANIMATION_TIME = 2.0f;
        yield return new WaitForSeconds(DEAD_ANIMATION_TIME);
        RestartScene();   
    }

    private void RestartScene()
    {
        LoadSceneWithLoadingUI(SceneManager.GetActiveScene().name);   
    }

    private void HandlePauseAction()
    {
        Debug.Assert(_settingsCanvas != null, "_settingsCanvas != null");
        Debug.Assert(_pauseCanvas != null, "_popupCanvas != null");
        
        if (IsFading)
        {
            return;
        }

        if (PopupHandler.Instance.IsPopupActive)
        {
            PopupHandler.Instance.ExecuteActionOnButtonClick(false);
            return;
        }

        if (IsSettingsCanvasActive)
        {
            ToggleSettingsCanvas();
            return;
        }
        
        TogglePauseCanvas();
    }

    private void TogglePauseCanvas()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            AudioPlayManager.Instance.PauseAllAudio();
        }
        else
        {
            AudioPlayManager.Instance.UnPauseAllAudio();   
        }
        _pauseCanvas.SetActive(_isPaused);
        ToggleCursorVisible();
        

        Time.timeScale = _isPaused ? 0.0f : 1.0f;
    }

    public void OnResumeButtonClick()
    {
        TogglePauseCanvas();
    }

    public void OnSettingsButtonClick()
    {
        ToggleSettingsCanvas();
    }

    public void OnQuitButtonClick()
    {
        PopupHandler.Instance.DisplayConfirmPopup(HandlePopupButtonAction, "메인으로", "메인 메뉴로 돌아가시겠습니까?", "예", "아니오");
    }

    private void HandlePopupButtonAction(bool isPositive)
    {
        if (!isPositive)
        {
            return;
        }

        TogglePauseCanvas();
        LoadSceneWithLoadingUI(SceneNames.MAIN_MENU);
    }

    private void ToggleSettingsCanvas()
    {
        _settingsCanvas.SetActive(!IsSettingsCanvasActive);
    }

    public void FadeIn(float duration, float delay = 0f, bool ignoreAudio = false)
    {
        StartCoroutine(FadeInRoutine(duration, delay, ignoreAudio));
    }

    public void FadeOut(float duration, float delay = 0f, bool ignoreAudio = false)
    {
        StartCoroutine(FadeOutRoutine(duration, delay, ignoreAudio));
    }

    private void GetSettingsValueAndApply()
    {
        // 해상도
        int width = PlayerPrefs.GetInt(PlayerPrefsKeyNames.RESOLUTION_WIDTH, Screen.width);
        int height = PlayerPrefs.GetInt(PlayerPrefsKeyNames.RESOLUTION_HEIGHT, Screen.height);
        
        var fullScreenMode = (FullScreenMode)PlayerPrefs.GetInt(PlayerPrefsKeyNames.DISPLAY_MODE,
            (int)FullScreenMode.ExclusiveFullScreen);
        
        int refreshRate = PlayerPrefs.GetInt(PlayerPrefsKeyNames.RESOLUTION_REFRESH_RATE,
            Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value));
        Screen.SetResolution(width, height, fullScreenMode, new RefreshRate()
        {
            numerator = (uint)refreshRate,
            denominator = 1U,
        });
        
        // 텍스처 품질
        QualitySettings.globalTextureMipmapLimit = PlayerPrefs.GetInt(PlayerPrefsKeyNames.TEXTURE_QUALITY, 0);
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
