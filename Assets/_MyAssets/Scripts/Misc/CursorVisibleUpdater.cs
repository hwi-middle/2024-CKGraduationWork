using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorVisibleUpdater : Singleton<CursorVisibleUpdater>
{
    private bool _isNeedCursorLock;
    
    private SceneManagerBase _sceneManagerBase;
    private PopupHandler _popupHandler;

    private void Awake()
    {
        _isNeedCursorLock = SceneManagerBase.Instance.IsNeedCursorLock;
        
        _sceneManagerBase = SceneManagerBase.Instance;
        _popupHandler = PopupHandler.Instance;
    }
    
    private void Update()
    {
        CheckAndUpdateCanvasState();
    }

    private void CheckAndUpdateCanvasState()
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

        if (_sceneManagerBase.IsOnSettingCanvas)
        {
            EnableCursorVisible();
            return;
        }

        if (_sceneManagerBase.IsOnPauseCanvas)
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
