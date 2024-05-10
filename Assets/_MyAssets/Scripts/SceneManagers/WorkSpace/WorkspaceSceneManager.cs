using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceSceneManager : SceneManagerBase
{
    private int _loopSfxSoundObjectID;
        
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
                SoundPlayManager.Instance.PlayOnceSfxSound(ESfxAudioClipIndex.TestSE);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                _loopSfxSoundObjectID = SoundPlayManager.Instance.PlayLoopSfxSound(ESfxAudioClipIndex.TestSE);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                SoundPlayManager.Instance.PlayBgmSound(EBgmAudioClipIndex.TestBGM);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SoundPlayManager.Instance.StopBgmSound();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SoundPlayManager.Instance.StopLoopSfxSound(_loopSfxSoundObjectID);
            }
        }
    }
}
