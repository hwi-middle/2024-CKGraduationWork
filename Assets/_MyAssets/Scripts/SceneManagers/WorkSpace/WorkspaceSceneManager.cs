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
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SoundPlayManager.Instance.PlaySfxSound(ESfxAudioClipIndex.TestSE);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SoundPlayManager.Instance.PlayBgmSound(EBgmAudioClipIndex.TestBGM);
            }
        }
    }
}
