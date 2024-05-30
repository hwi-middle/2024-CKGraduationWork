using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSoundEvents : MonoBehaviour
{
    private readonly List<ESfxAudioClipIndex> _playerWalkSounds = new();
    private readonly List<ESfxAudioClipIndex> _playerRunningSounds = new();
    private readonly List<ESfxAudioClipIndex> _playerRunningBreathSounds = new();

    private const int BREATH_CYCLE_COUNT = 2;
    private int _cycleCount = BREATH_CYCLE_COUNT;

    private void Awake()
    {
        InitPlayerWalkSound();
        InitPlayerRunningSound();
        InitPlayerRunningBreathSound();
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

    private void InitPlayerRunningBreathSound()
    {
        _playerRunningBreathSounds.Add(ESfxAudioClipIndex.Player_Running_Breath1);
        _playerRunningBreathSounds.Add(ESfxAudioClipIndex.Player_Running_Breath2);
    }
    
    public void PlayFootStepSound()
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(_playerWalkSounds[Random.Range(0, _playerWalkSounds.Count)]);
    }

    public void PlayRunningSound()
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(_playerRunningSounds[Random.Range(0, _playerRunningSounds.Count)]);
    }
    
    public void PlayRunningBreathSound()
    {
        if (_cycleCount < BREATH_CYCLE_COUNT)
        {
            _cycleCount++;
            return;
        }

        _cycleCount = 0;
        AudioPlayManager.Instance.PlayOnceSfxAudio(_playerRunningBreathSounds[Random.Range(0, _playerRunningBreathSounds.Count)]);
    }
}
