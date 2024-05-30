using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSoundEvents : MonoBehaviour
{
    private List<ESfxAudioClipIndex> _playerWalkSounds = new();
    private List<ESfxAudioClipIndex> _playerRunningSounds = new();

    private void Awake()
    {
        InitPlayerWalkSound();
        InitPlayerRunningSound();
    }

    private void InitPlayerWalkSound()
    {
        _playerWalkSounds.Add(ESfxAudioClipIndex.Player_Walk1);
        _playerWalkSounds.Add(ESfxAudioClipIndex.Player_Walk2);
        _playerWalkSounds.Add(ESfxAudioClipIndex.Player_Walk3);
    }
    
    private void InitPlayerRunningSound()
    {
        _playerRunningSounds.Add(ESfxAudioClipIndex.Player_Running1);
        _playerRunningSounds.Add(ESfxAudioClipIndex.Player_Running2);
        _playerRunningSounds.Add(ESfxAudioClipIndex.Player_Running3);
    }
    
    public void PlayFootStepSound()
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(_playerWalkSounds[Random.Range(0, _playerWalkSounds.Count)]);
    }

    public void PlayRunningSound()
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(_playerRunningSounds[Random.Range(0, _playerRunningSounds.Count)]);
    }
}
