using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneManager : SceneManagerBase
{
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;
    
    [SerializeField] private Image[] _backgroundImages;
    [SerializeField] private float _bgScrollSpeed;   
    [SerializeField] private Image[] _foregroundImages;
    [SerializeField] private float _fgScrollSpeed;
    
    protected override void Awake()
    {
        base.Awake();
        
        _newGameButton.onClick.AddListener(OnNewGameButtonClick);
        _continueButton.onClick.AddListener(OnCreditButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
        
        Debug.Assert(_backgroundImages.Length >= 2);
        Debug.Assert(_foregroundImages.Length >= 2);
    }

    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);

        //_continueButton.interactable = CheckPointRootHandler.Instance.HasSavedCheckPointData;
        AudioPlayManager.Instance.PlayBgmAudio(EBgmAudioClipIndex.MainMenu_BGM);
    }

    protected override void Update()
    {
        base.Update();

        if (IsDebugMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                PopupHandler.Instance.DisplayErrorPopup(HandleErrorPopupButtonAction, "Test Error Title", "Error Popup 테스트 입니다.", "확인");
            }
        }

        foreach (Image bg in _backgroundImages)
        {
            bg.rectTransform.anchoredPosition -= new Vector2(_bgScrollSpeed, 0.0f) * Time.deltaTime;
            if(bg.rectTransform.anchoredPosition.x < -bg.rectTransform.rect.width)
            {
                bg.rectTransform.anchoredPosition += new Vector2(bg.rectTransform.rect.width * _backgroundImages.Length, 0.0f);
            }
        }
        
        foreach (Image fg in _foregroundImages)
        {
            fg.rectTransform.anchoredPosition -= new Vector2(_fgScrollSpeed, 0.0f) * Time.deltaTime;
            if(fg.rectTransform.anchoredPosition.x < -fg.rectTransform.rect.width / 2)
            {
                fg.rectTransform.anchoredPosition += new Vector2(fg.rectTransform.rect.width   * _foregroundImages.Length, 0.0f);
            }
        }
    }

    public void OnNewGameButtonClick()
    {
        // First Game Scene Start
        PopupHandler.Instance.DisplayWarningPopup(HandleNewGameButtonAction, "게임을 새로 시작하시겠습니까?",
            "(세이브한 파일이 초기화 됩니다.)", "확인", "취소");
    }

    private void HandleNewGameButtonAction(bool isPositive)
    {
        if (!isPositive)
        {
            return;
        }

        LoadSceneWithLoadingUI(SceneNames.DEMO_PLAY);
    }

    private void OnCreditButtonClick()
    {
        PopupHandler.Instance.DisplayConfirmPopup(HandleCreditButtonAction, "크레딧을 보시겠습니까?", "", "예", "아니오");
    }
    
    private void HandleCreditButtonAction(bool isPositive)
    {
        if (!isPositive)
        {
            return;
        }

        LoadSceneWithLoadingUI(SceneNames.CREDIT);
    }

    public void OnExitButtonClick()
    {
        PopupHandler.Instance.DisplayConfirmPopup(HandleExitPopupButtonAction, "게임을 종료하시겠습니까?", "", "예", "아니오");
    }
    
    private void HandleErrorPopupButtonAction(bool isPositive)
    {
        // Error에 대한 처리를 여기서 작성합니다.
    }

    private void HandleExitPopupButtonAction(bool isPositive)
    {
        if (!isPositive)
        {
            return;
        }

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(1);
#endif
    }
}
