using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxAudioObject : MonoBehaviour
{
    private AudioSource _audioSource;
    
    public AudioClip Clip => _audioSource.clip;
    
    private bool _isPaused;
    public bool IsLoop => _audioSource.loop;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _isPaused = false;
    }

    private void Update()
    {
        CheckNonLoopSfxPlayingAndReturnObject();
    }
    
    private void CheckNonLoopSfxPlayingAndReturnObject()
    {
        if (_isPaused || IsLoop || _audioSource.isPlaying)
        {
            return;
        }
        
        Stop();
    }

    public void Play(AudioClip clip, ESfxPlayType sfxPlayType)
    {
        _audioSource.clip = clip;
        _audioSource.loop = sfxPlayType == ESfxPlayType.Loop;
        
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
        gameObject.SetActive(false);
    }

    public void Pause()
    {
        _isPaused = true;
        _audioSource.Pause();
    }

    public void UnPause()
    {
        _isPaused = false;
        _audioSource.UnPause();
    }
} 