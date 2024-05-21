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
    
    protected override void Awake()
    {
        base.Awake();
        
        _newGameButton.onClick.AddListener(OnNewGameButtonClick);
        _continueButton.onClick.AddListener(OnContinueButton);
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);

        _continueButton.interactable = CheckPointRootHandler.Instance.HasSavedCheckPointData;
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
    }

    public void OnNewGameButtonClick()
    {
        // First Game Scene Start
        PopupHandler.Instance.DisplayWarningPopup(HandleNewGameButtonAction, "새 게임",
            "새 게임을 시작하시겠습니까?\n저장된 데이터는 모두 사라집니다.", "예", "아니오");
    }

    private void HandleNewGameButtonAction(bool isPositive)
    {
        if (!isPositive)
        {
            return;
        }

        LoadSceneWithLoadingUI(SceneNames.DEMO_PLAY);
    }

    public void OnContinueButton()
    {
        // Saved Game Scene Start
        PopupHandler.Instance.DisplayInfoPopup(HandleContinueButtonAction, "이어 하기 (준비 중)", "해당 기능은 구현 중 입니다.", "확인");
    }

    private void HandleContinueButtonAction(bool isPositive)
    {
    }

    public void OnExitButtonClick()
    {
        PopupHandler.Instance.DisplayConfirmPopup(HandleExitPopupButtonAction, "게임 종료", "게임을 종료하시겠습니까?", "예", "아니오");
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
