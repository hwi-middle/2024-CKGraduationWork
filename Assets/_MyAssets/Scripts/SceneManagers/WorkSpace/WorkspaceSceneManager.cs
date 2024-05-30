using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceSceneManager : SceneManagerBase
{
    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);
        AudioPlayManager.Instance.PlayLoopSfxAudio(ESfxAudioClipIndex.ZoneA_AMB_Loop);
    }

    protected override void Update()
    {
        base.Update();
        if (IsDebugMode)
        {
            
        }
    }

    private void HandleTutorialPopupButtonAction(bool isPositive)
    {
        TogglePause();
    }
}
