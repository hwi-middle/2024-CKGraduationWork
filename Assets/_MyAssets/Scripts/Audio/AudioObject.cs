using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioObject : MonoBehaviour
{
    public AudioClip Clip => _audioSource.clip;
    private int _loopSfxAudioObjectID;
    
    public int LoopSfxAudioObjectID => _loopSfxAudioObjectID;
    
    private ESfxAudioClipIndex _sfxClipIndex;
    private EBgmAudioClipIndex _bgmClipIndex;
    private AudioSource _audioSource;

    private bool _isPaused;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _isPaused = false;
    }

    private void Update()
    {
        CheckSfxIsPlayingAndReturnObject();
    }
    
    // 효과음 재생이 끝났는지 확인하고 반환
    private void CheckSfxIsPlayingAndReturnObject()
    {
        if (_isPaused)
        {
            return;
        }
        
        if (_audioSource.isPlaying || _sfxClipIndex == ESfxAudioClipIndex.None)
        {
            return;
        }
        
        _sfxClipIndex = ESfxAudioClipIndex.None;
        _audioSource.clip = null;
        gameObject.SetActive(false);
    }

    public void PlaySfxAudio(ESfxAudioClipIndex clipIndex, AudioClip clip, EPlayType playType)
    {
        _sfxClipIndex = clipIndex;
        _audioSource.clip = clip;
        _audioSource.loop = playType == EPlayType.Loop;
        _loopSfxAudioObjectID = playType == EPlayType.Loop ? Random.Range(int.MinValue, int.MaxValue) : int.MaxValue;
        
        _audioSource.Play();
    }

    public void PlayBgmAudio(EBgmAudioClipIndex clipIndex, AudioClip clip)
    {
        _bgmClipIndex = clipIndex;
        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void StopAudio()
    {
        _audioSource.Stop();
        
        if (_bgmClipIndex == EBgmAudioClipIndex.None)
        {
            _sfxClipIndex = ESfxAudioClipIndex.None;
        }
        else
        {
            _bgmClipIndex = EBgmAudioClipIndex.None;
        }
        
        _audioSource.clip = null;
        gameObject.SetActive(false);
    }

    public void PauseAudio()
    {
        _isPaused = true;
        _audioSource.Pause();
    }

    public void UnPauseAudio()
    {
        _isPaused = false;
        _audioSource.UnPause();
    }
} 