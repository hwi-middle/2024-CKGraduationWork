using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeethAnimationEvents : MonoBehaviour
{
    public void OnAnimationStart()
    {
        AudioPlayManager.Instance.PlayLoopSfxAudio(ESfxAudioClipIndex.Noise_Item2, transform);
    }
    
    public void OnTeethAnimationEnd()
    {
        AudioPlayManager.Instance.StopLoopSfxAudio(ESfxAudioClipIndex.Noise_Item2);
        Destroy(transform.parent.gameObject);
    }
}
