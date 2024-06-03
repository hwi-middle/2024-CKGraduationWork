using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TeethAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject _smokeEffectObject;

    private void Start()
    {
        _smokeEffectObject.SetActive(false);
    }

    public void OnAnimationStart()
    {
        AudioPlayManager.Instance.PlayLoopSfxAudio(ESfxAudioClipIndex.Noise_Item2, transform);
        _smokeEffectObject.SetActive(true);
    }
    
    public void OnTeethAnimationEnd()
    {
        AudioPlayManager.Instance.StopLoopSfxAudio(ESfxAudioClipIndex.Noise_Item2);
        Destroy(transform.parent.gameObject);
    }
}
