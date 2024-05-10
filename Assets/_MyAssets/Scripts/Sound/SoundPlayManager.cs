using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum ESoundType
{
    Bgm,
    Sfx
}

public enum EPlayType
{
    PlayOnce,
    Loop
}

public class SoundPlayManager : Singleton<SoundPlayManager>
{
    [SerializeField] SoundClipData _soundClipData;
    
    // 효과음 오브젝트 프리팹
    private GameObject _soundObjectPrefab;
    
    // 배경음 오브젝트 풀
    private SoundObject _bgmSoundObject;
    // 효과음 오브젝트 풀
    private List<SoundObject> _allocatedSfxSoundObjects = new();
    
    // string 할당을 단 한번만 하기 위한 Dictionary
    private Dictionary<int, AudioClip> _cachedSfxClips = new();
    private Dictionary<int, AudioClip> _cachedBgmClips = new();
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (_soundObjectPrefab == null)
        {
            _soundObjectPrefab = Resources.Load<GameObject>("Sound/SoundObject");
        }
        
        if (_allocatedSfxSoundObjects.Count != 0)
        {
            _allocatedSfxSoundObjects.Clear();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        GameObject bgmSoundObject = Instantiate(_soundObjectPrefab, transform);
        bgmSoundObject.name = "BGMSoundObject";
        _bgmSoundObject = bgmSoundObject.GetComponent<SoundObject>();
        bgmSoundObject.SetActive(false);
        
        for (int i = 0; i <= _soundClipData.maxSoundObjectCount; i++)
        {
            GameObject soundObject = Instantiate(_soundObjectPrefab, transform);
            soundObject.name = "SoundObject_" + i;
            soundObject.SetActive(false);
            _allocatedSfxSoundObjects.Add(soundObject.GetComponent<SoundObject>());
        }
    }

    private SoundObject GetAvailableSoundObject()
    {
        foreach(SoundObject soundObject in _allocatedSfxSoundObjects)
        {
            if (!soundObject.gameObject.activeSelf)
            {
                return soundObject;
            }
        }

        return null;
    }
    
    private AudioClip GetClip(ESoundType soundType, string clipName)
    {
        List<AudioClip> clipList = soundType == ESoundType.Bgm ? _soundClipData.bgmClipList : _soundClipData.sfxClipList;
        return clipList.Find(clip => clip.name == clipName);
    }

    public void PlayOnceSfxSound(ESfxAudioClipIndex clip)
    {
        if (!_cachedSfxClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(ESoundType.Sfx, clip.ToString());
            _cachedSfxClips.Add((int)clip, audioClip);
        }

        SoundObject availableObject = GetAvailableSoundObject();
        availableObject.gameObject.SetActive(true);
        availableObject.PlaySfxSound(clip, audioClip, EPlayType.PlayOnce);
    }

    public int PlayLoopSfxSound(ESfxAudioClipIndex clip)
    {
        if(!_cachedSfxClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(ESoundType.Sfx, clip.ToString());
            _cachedSfxClips.Add((int)clip, audioClip);
        }

        Debug.Assert(audioClip.name != null);

        int playingLoopSfxID = CheckIsPlayingLoopSfx(audioClip);
        if (playingLoopSfxID != int.MaxValue)
        {
            return playingLoopSfxID;
        }
        
        SoundObject availableObject = GetAvailableSoundObject();
        availableObject.gameObject.SetActive(true);
        availableObject.PlaySfxSound(clip, audioClip, EPlayType.Loop);
        return availableObject.LoopSfxSoundObjectID;
    }

    private int CheckIsPlayingLoopSfx(AudioClip audioClip)
    {
        foreach (SoundObject soundObject in _allocatedSfxSoundObjects)
        {
            if (!soundObject.gameObject.activeSelf || soundObject.LoopSfxSoundObjectID == int.MaxValue)
            {
                continue;
            }
            
            if (soundObject.Clip.name == audioClip.name)
            {
                return soundObject.LoopSfxSoundObjectID;
            }
        }

        return int.MaxValue;
    }

    public void PlayBgmSound(EBgmAudioClipIndex clip)
    {
        if (!_cachedBgmClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(ESoundType.Bgm, clip.ToString());
            _cachedBgmClips.Add((int)clip, audioClip);
        }

        if (_bgmSoundObject.gameObject.activeSelf && _bgmSoundObject.Clip == audioClip)
        {
            return;
        }
        
        _bgmSoundObject.gameObject.SetActive(true);
        _bgmSoundObject.PlayBgmSound(clip, audioClip);
    }
    
    public void StopLoopSfxSound(int soundObjectID)
    {
        foreach (SoundObject currentSoundObject in _allocatedSfxSoundObjects)
        {
            if (!currentSoundObject.gameObject.activeSelf
                || currentSoundObject.LoopSfxSoundObjectID != soundObjectID)
            {
                continue;
            }

            currentSoundObject.StopSound();
            break;
        }
    }

    public void StopBgmSound()
    {
        if (_bgmSoundObject.gameObject.activeSelf)
        {
            _bgmSoundObject.StopSound();
        }
    }
}
