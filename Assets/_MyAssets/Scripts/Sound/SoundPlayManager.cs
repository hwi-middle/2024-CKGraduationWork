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

    private GameObject _soundObject;
    private SoundObject _bgmSoundObject;
    private List<SoundObject> _soundObjectList = new();
    
    private void Awake()
    {
        Init();      
    }

    private void Init()
    {
        if (_soundObject == null)
        {
            _soundObject = Resources.Load<GameObject>("Sound/SoundObject");
        }
        
        if (_soundObjectList.Count != 0)
        {
            _soundObjectList.Clear();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        GameObject bgmSoundObject = Instantiate(_soundObject, transform);
        bgmSoundObject.name = "BGMSoundObject";
        _bgmSoundObject = bgmSoundObject.GetComponent<SoundObject>();
        
        for (int i = 0; i <= _soundClipData.maxSoundObjectCount; i++)
        {
            GameObject soundObject = Instantiate(_soundObject, transform);
            soundObject.name = "SoundObject_" + i;
            soundObject.SetActive(false);
            _soundObjectList.Add(soundObject.GetComponent<SoundObject>());
        }
    }

    private SoundObject GetAvailableSoundObject()
    {
        foreach(SoundObject soundObject in _soundObjectList)
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

    public SoundObject PlaySfxSound(string clipName)
    {
        SoundObject availableObject = GetAvailableSoundObject();
        availableObject.gameObject.SetActive(true);
        availableObject.PlaySound(GetClip(ESoundType.Sfx, clipName), EPlayType.PlayOnce);
        return availableObject;
    }

    public SoundObject PlayBgmSound(string clipName)
    {
        _bgmSoundObject.gameObject.SetActive(true);
        _bgmSoundObject.PlaySound(GetClip(ESoundType.Bgm, clipName), EPlayType.Loop);
        return _bgmSoundObject;
    }
}
