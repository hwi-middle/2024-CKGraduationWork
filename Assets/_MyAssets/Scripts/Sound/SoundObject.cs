using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckAudioStoppedAndReturnObject();
    }
    
    private void CheckAudioStoppedAndReturnObject()
    {
        if (_audioSource.isPlaying || _audioSource.loop)
        {
            return;
        }

        _audioSource.clip = null;
        gameObject.SetActive(false);
    }

    public void PlaySound(AudioClip clip, EPlayType playType)
    {
        _audioSource.clip = clip;
        _audioSource.loop = playType == EPlayType.Loop;
        _audioSource.Play();
    }

    public void StopSound()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}
