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

    public SoundObject PlaySfxSound(ESfxAudioClipIndex clip)
    {
        if (!_cachedSfxClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(ESoundType.Sfx, clip.ToString());
            _cachedSfxClips.Add((int)clip, audioClip);
        }

        SoundObject availableObject = GetAvailableSoundObject();
        availableObject.gameObject.SetActive(true);
        availableObject.PlaySound(audioClip, EPlayType.PlayOnce);
        return availableObject;
    }

    public SoundObject PlayBgmSound(EBgmAudioClipIndex clip)
    {
        if (!_cachedBgmClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(ESoundType.Bgm, clip.ToString());
            _cachedBgmClips.Add((int)clip, audioClip);
        }
        
        _bgmSoundObject.gameObject.SetActive(true);
        _bgmSoundObject.PlaySound(audioClip, EPlayType.Loop);
        return _bgmSoundObject;
    }
}
