using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLockModeUpdater : Singleton<CursorLockModeUpdater>
{
    private bool _isNeedCursorLock;
    
    private SceneManagerBase _sceneManagerBase;
    private PopupHandler _popupHandler;

    private void Awake()
    {
        _sceneManagerBase = SceneManagerBase.Instance;
        _isNeedCursorLock = _sceneManagerBase.IsNeedCursorLock;
        
        _popupHandler = PopupHandler.Instance;
    }
    
    private void Update()
    {
        UpdateCursorVisibility();
    }

    private void UpdateCursorVisibility()
    {
        if (!_isNeedCursorLock)
        {
            EnableCursorVisible();
            return;
        }

        if (_popupHandler.IsPopupActive)
        {
            EnableCursorVisible();
            return;
        }

        if (_sceneManagerBase.IsSettingsCanvasActive)
        {
            EnableCursorVisible();
            return;
        }

        if (_sceneManagerBase.IsPauseCanvasActive)
        {
            EnableCursorVisible();
            return;
        }
        
        DisableCursorVisible();
    }

    private void EnableCursorVisible()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void DisableCursorVisible()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
