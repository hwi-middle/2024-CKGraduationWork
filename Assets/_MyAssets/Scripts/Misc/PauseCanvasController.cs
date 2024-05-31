using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseCanvasController : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _quitButton;

    private void OnEnable()
    {
        _resumeButton.onClick.AddListener(HandleResumeButton);
        _optionsButton.onClick.AddListener(HandleSettingsButton);
        _menuButton.onClick.AddListener(HandleMenuButton);
        _quitButton.onClick.AddListener(HandleQuitButton);

        bool isMainMenu = SceneManager.GetActiveScene().name.Equals(SceneNames.MAIN_MENU);
        _menuButton.gameObject.SetActive(!isMainMenu);
    }
    
    private void OnDisable()
    {
        _resumeButton.onClick.RemoveListener(HandleResumeButton);
        _optionsButton.onClick.RemoveListener(HandleSettingsButton);
        _menuButton.onClick.RemoveListener(HandleMenuButton);
    }
    
    private void HandleResumeButton()
    {
        SceneManagerBase.Instance.OnResumeButtonClick();
    }
    
    private void HandleSettingsButton()
    {
        SceneManagerBase.Instance.OnSettingsButtonClick();
    }
    
    private void HandleMenuButton()
    {
        SceneManagerBase.Instance.OnQuitButtonClick();
    }

    private void HandleQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(0);       
#endif
    }
}