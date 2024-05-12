using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceSceneManager : SceneManagerBase
{
    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);
    }

    protected override void Update()
    {
        base.Update();
        if (IsDebugMode)
        {
            if(Input.GetKeyDown(KeyCode.Alpha0))
            {
                PopupHandler.Instance.DisplayTutorialPopup(HandleTutorialPopupButtonAction,
                    "Test Tutorial Title", "Tutorial Popup 테스트 입니다.", "확인");
            }
        }
    }

    private void HandleTutorialPopupButtonAction(bool isPositive)
    {
        TogglePause();
    }
}
