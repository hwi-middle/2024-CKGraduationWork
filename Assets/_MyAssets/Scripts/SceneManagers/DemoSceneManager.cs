using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DemoSceneManager : SceneManagerBase
{
    private GameObject _settingCanvas;
    private bool _isPaused;

    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);
    }

    protected override void Update()
    {
        base.Update();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started || IsFading)
        {
            return;
        }

        _isPaused = !_isPaused;

        if (_isPaused)
        {
            _settingCanvas = Instantiate(Resources.Load<GameObject>("SettingCanvas"));

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            Time.timeScale = 0.0f;
            return;
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        Destroy(_settingCanvas);
        Time.timeScale = 1.0f;
    }

    public void OnStartButton()
    {
        LoadSceneWithLoadingUI("DemoPlay");
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(1);
#endif
    }
}
