using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasController : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;

    private void OnEnable()
    {
        _resumeButton.onClick.AddListener(HandleResumeButton);
        _optionsButton.onClick.AddListener(HandleOptionsButton);
        _quitButton.onClick.AddListener(HandleQuitButton);
        
        _quitButton.gameObject.SetActive(!SceneManagerBase.Instance.IsMainMenu);
    }
    
    private void OnDisable()
    {
        _resumeButton.onClick.RemoveListener(HandleResumeButton);
        _optionsButton.onClick.RemoveListener(HandleOptionsButton);
        _quitButton.onClick.RemoveListener(HandleQuitButton);
    }
    
    private void HandleResumeButton()
    {
        SceneManagerBase.Instance.OnResumeButtonClick();
    }
    
    private void HandleOptionsButton()
    {
        SceneManagerBase.Instance.OnOptionsButtonClick();
    }
    
    private void HandleQuitButton()
    {
        SceneManagerBase.Instance.OnQuitButtonClick();
    }
}
