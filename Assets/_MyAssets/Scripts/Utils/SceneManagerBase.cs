using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

public abstract class SceneManagerBase : Singleton<SceneManagerBase>
{
    [SerializeField] private bool _isDebugMode;
    public bool IsDebugMode => _isDebugMode;
    
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private InputActionAsset _actionAsset;
    [SerializeField] private bool _cursorLock;
    private SceneFadeManager _fadeManager;
    private GameObject _settingCanvas;
    private Transform _settingCanvasTabGroup;
    private Transform _settingCanvasTabPages;

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
        
        GetSettingsValueAndApply();

    }
    
    protected virtual void OnEnable()
    {
        _inputData.pauseEvent += HandlePauseAction;
        _inputData.tabMoveEvent += HandleTabMoveAction;
        _inputData.settingSubmitEvent += HandleSubmitAction;
        
        _inputData.backEvent += HandleBackAction;
    }

    protected void OnDisable()
    {
        _inputData.pauseEvent -= HandlePauseAction;
        _inputData.tabMoveEvent -= HandleTabMoveAction;
        _inputData.settingSubmitEvent -= HandleSubmitAction;
        
        _inputData.backEvent -= HandleBackAction;
    }

    protected virtual void Start()
    {
        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
        _fadeManager = SceneFadeManager.Instance;
        _fadeManager.GetComponent<Image>().enabled = true;

        _settingCanvas = Instantiate(Resources.Load<GameObject>("SettingCanvas"));
        _settingCanvasTabGroup = _settingCanvas.transform.GetChild(1);
        _settingCanvasTabPages = _settingCanvas.transform.GetChild(2);
        _settingCanvas.SetActive(false);
        
        InstantiateEventSystem();
        
        _isPaused = false;
    }

    private void InstantiateEventSystem()
    {
        EventSystem currentEventSystem = EventSystem.current;
        if (currentEventSystem != null)
        {
            Destroy(currentEventSystem.gameObject);
        }

        Instantiate(Resources.Load<GameObject>("EventSystem"));
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

        PlayerInputData.EInputMap inputMap;
        bool isHiding = PlayerMove.Instance.CheckPlayerState(EPlayerState.Hide);

        if (_isPaused)
        {
            inputMap = PlayerInputData.EInputMap.SettingAction;
        }
        else
        {
           inputMap = isHiding ? PlayerInputData.EInputMap.HideAction : PlayerInputData.EInputMap.PlayerAction; 
        }

        PlayerInputData.ChangeInputMap(inputMap);
    }

    private void HandleTabMoveAction(float value)
    {
        int childCount = _settingCanvasTabPages.childCount;
        int activeChildIndex = -1;
        for (int i = 0; i < childCount; i++)
        {
            if (!_settingCanvasTabPages.GetChild(i).gameObject.activeSelf)
            {
                continue;
            }
            
            activeChildIndex = i;
            break;
        }

        int inputValue = (int)value;

        switch (inputValue)
        {
            case -1:
                activeChildIndex = activeChildIndex != 0 ? activeChildIndex - 1 : 0;
                break;
            case 1:
                activeChildIndex = activeChildIndex != childCount - 1 ? activeChildIndex + 1 : childCount - 1;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        Transform selectedHeader = _settingCanvasTabGroup.GetChild(activeChildIndex);
        
        selectedHeader.GetComponent<TabHeader>().OnClick();
    }

    private void HandleSubmitAction()
    {
        int childCount = _settingCanvasTabGroup.childCount;
        for (int i = 0; i < childCount; i++)
        {
            _settingCanvasTabGroup.GetChild(i).GetComponent<Button>().interactable = false;
        }

        foreach (Transform tab in _settingCanvasTabPages)
        {
            if (tab.gameObject.activeSelf)
            {
                TabPage tabPage = tab.GetComponent<TabPage>();
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(tabPage.FirstSelected);
                break;
            }
        }

        _settingCanvas.transform.GetChild(_settingCanvas.transform.childCount - 1).gameObject.SetActive(false);
        SetInputSystemUIInputModule();
        PlayerInputData.ChangeInputMap(PlayerInputData.EInputMap.SettingDetailAction);
    }

    private void HandleBackAction()
    {
        GameObject lastChildObject = _settingCanvas.transform.GetChild(_settingCanvas.transform.childCount - 1).gameObject;
        if (lastChildObject.name == "Blocker")
        {
            return;
        }
        
        int childCount = _settingCanvasTabGroup.childCount;
        for (int i = 0; i < childCount; i++)
        {
            _settingCanvasTabGroup.GetChild(i).GetComponent<Button>().interactable = true;
        }

        _settingCanvas.transform.GetChild(_settingCanvas.transform.childCount - 1).gameObject.SetActive(true);
        UnsetInputSystemUIInputModule();
        PlayerInputData.ChangeInputMap(PlayerInputData.EInputMap.SettingAction);
    }

    private void SetInputSystemUIInputModule()
    {
        InputSystemUIInputModule module = EventSystem.current.transform.GetComponent<InputSystemUIInputModule>();

        InputAction moveAction = _actionAsset.FindAction("DetailMove");
        InputAction submitAction = _actionAsset.FindAction("DetailSubmit");
        
        module.move = InputActionReference.Create(moveAction);
        module.submit = InputActionReference.Create(submitAction);
    }
    
    private void UnsetInputSystemUIInputModule()
    {
        InputSystemUIInputModule module = EventSystem.current.transform.GetComponent<InputSystemUIInputModule>();
        module.move = null;
        module.submit = null;
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
