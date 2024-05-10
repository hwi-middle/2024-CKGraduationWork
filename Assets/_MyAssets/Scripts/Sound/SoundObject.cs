using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundObject : MonoBehaviour
{
    public AudioClip Clip => _audioSource.clip;
    public bool IsLoop => _audioSource.loop;
    
    private int _loopSfxSoundObjectID;
    public int LoopSfxSoundObjectID => _loopSfxSoundObjectID;
    
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

    public void PlaySfxSound(ESfxAudioClipIndex clipIndex, AudioClip clip, EPlayType playType)
    {
        _sfxClipIndex = clipIndex;
        _audioSource.clip = clip;
        _audioSource.loop = playType == EPlayType.Loop;
        _loopSfxSoundObjectID = playType == EPlayType.Loop ? Random.Range(int.MinValue, int.MaxValue) : int.MaxValue;
        
        _audioSource.Play();
    }

    public void PlayBgmSound(EBgmAudioClipIndex clipIndex, AudioClip clip)
    {
        _bgmClipIndex = clipIndex;
        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void StopSound()
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

    public void PauseSound()
    {
        _isPaused = true;
        _audioSource.Pause();
    }

    public void UnPauseSound()
    {
        _isPaused = false;
        _audioSource.UnPause();
    }
} 