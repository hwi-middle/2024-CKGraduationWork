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
        LoadSceneWithLoadingUI("PopupScene");
    }

    public void OnContinueButton()
    {
        // Saved Game Scene Start
        LoadSceneWithLoadingUI("PopupScene");
    }

    public void OnExitButton()
    {
        PopupHandler.Instance.ShowPopup("MenuQuit");
        PopupHandler.Instance.SubscribeButtonAction(HandlePopupButtonAction);
    }
    
    
    private void HandlePopupButtonAction(bool isPositive)
    {
        if (isPositive)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit(1);
#endif
        }
        
        PopupHandler.Instance.UnsubscribeButtonAction(HandlePopupButtonAction);
    }
}
