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
            if (Input.GetKeyDown(KeyCode.F1))
            {
                AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.Sfx_1);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                AudioPlayManager.Instance.PlayLoopSfxAudio(ESfxAudioClipIndex.Sfx_1);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                AudioPlayManager.Instance.PlayBgmAudio(EBgmAudioClipIndex.Bgm_1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                AudioPlayManager.Instance.StopBgmAudio();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                AudioPlayManager.Instance.StopLoopSfxAudio(ESfxAudioClipIndex.Sfx_1);
            }
        }
    }
}
