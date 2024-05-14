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
    }

    private void HandleTutorialPopupButtonAction(bool isPositive)
    {
        TogglePause();
    }
}
