using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuSceneManager : SceneManagerBase
{
    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);
    }

    protected override void Update()
    {
        base.Update();
    }

    public void OnNewGameButton()
    {
        // First Game Scene Start
        PopupHandler.Instance.FloatWarningPopup(HandleNewGameButtonAction, "새 게임",
            "새 게임을 시작하시겠습니까?\n저장된 데이터는 모두 사라집니다.", "예", "아니오");
    }

    private void HandleNewGameButtonAction(bool isPositive)
    {
        if (!isPositive)
        {
            return;
        }

        LoadSceneWithLoadingUI("PopupScene");
    }

    public void OnContinueButton()
    {
        // Saved Game Scene Start
        PopupHandler.Instance.FloatInfoPopup(HandleContinueButtonAction, "이어 하기 (준비 중)", "해당 기능은 구현 중 입니다.", "확인");
    }

    private void HandleContinueButtonAction(bool isPositive)
    {
    }

    public void OnExitButton()
    {
        PopupHandler.Instance.FloatConfirmPopup(HandleExitPopupButtonAction, "게임 종료", "게임을 종료하시겠습니까?", "예", "아니오");
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
