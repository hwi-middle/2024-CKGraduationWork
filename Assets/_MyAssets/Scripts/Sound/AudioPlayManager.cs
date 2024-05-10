using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public enum EAudioType
{
    Bgm,
    Sfx
}

public enum EPlayType
{
    PlayOnce,
    Loop
}

public class AudioPlayManager : Singleton<AudioPlayManager>
{
    [SerializeField] AudioClipData _audioClipData;
    
    // 효과음 오브젝트 프리팹
    private GameObject _audioObjectPrefab;
    
    // 배경음 오브젝트 풀
    private AudioObject _bgmAudioObject;
    // 효과음 오브젝트 풀
    private List<AudioObject> _allocatedSfxAudioObjects = new();
    
    // string 할당을 단 한번만 하기 위한 Dictionary
    private Dictionary<int, AudioClip> _cachedSfxClips = new();
    private Dictionary<int, AudioClip> _cachedBgmClips = new();
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (_audioObjectPrefab == null)
        {
            _audioObjectPrefab = Resources.Load<GameObject>("Sound/AudioObject");
        }
        
        if (_allocatedSfxAudioObjects.Count != 0)
        {
            _allocatedSfxAudioObjects.Clear();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        GameObject bmgAudioObject = Instantiate(_audioObjectPrefab, transform);
        bmgAudioObject.name = "BGMAudioObject";
        _bgmAudioObject = bmgAudioObject.GetComponent<AudioObject>();
        bmgAudioObject.SetActive(false);
        
        for (int i = 0; i <= _audioClipData.maxAudioObjectCount; i++)
        {
            GameObject audioObject = Instantiate(_audioObjectPrefab, transform);
            audioObject.name = "SfxAudioObject_" + i;
            audioObject.SetActive(false);
            _allocatedSfxAudioObjects.Add(audioObject.GetComponent<AudioObject>());
        }
    }

    private AudioObject GetAvailableAudioObject()
    {
        foreach(AudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf)
            {
                return audioObject;
            }
        }

        return null;
    }
    
    private AudioClip GetClip(EAudioType audioType, string clipName)
    {
        List<AudioClip> clipList = audioType == EAudioType.Bgm ? _audioClipData.bgmClipList : _audioClipData.sfxClipList;
        return clipList.Find(clip => clip.name == clipName);
    }

    public void PlayOnceSfxAudio(ESfxAudioClipIndex clip)
    {
        if (!_cachedSfxClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(EAudioType.Sfx, clip.ToString());
            _cachedSfxClips.Add((int)clip, audioClip);
        }

        AudioObject availableObject = GetAvailableAudioObject();
        availableObject.gameObject.SetActive(true);
        availableObject.PlaySfxAudio(clip, audioClip, EPlayType.PlayOnce);
    }

    public int PlayLoopSfxAudio(ESfxAudioClipIndex clip)
    {
        if(!_cachedSfxClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(EAudioType.Sfx, clip.ToString());
            _cachedSfxClips.Add((int)clip, audioClip);
        }

        Debug.Assert(audioClip.name != null);

        int playingLoopSfxID = CheckIsPlayingLoopSfx(audioClip);
        if (playingLoopSfxID != int.MaxValue)
        {
            return playingLoopSfxID;
        }
        
        AudioObject availableObject = GetAvailableAudioObject();
        availableObject.gameObject.SetActive(true);
        availableObject.PlaySfxAudio(clip, audioClip, EPlayType.Loop);
        return availableObject.LoopSfxAudioObjectID;
    }

    private int CheckIsPlayingLoopSfx(AudioClip audioClip)
    {
        foreach (AudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf || audioObject.LoopSfxAudioObjectID == int.MaxValue)
            {
                continue;
            }
            
            if (audioObject.Clip.name == audioClip.name)
            {
                return audioObject.LoopSfxAudioObjectID;
            }
        }

        return int.MaxValue;
    }

    public void PlayBgmAudio(EBgmAudioClipIndex clip)
    {
        if (!_cachedBgmClips.TryGetValue((int)clip, out AudioClip audioClip))
        {
            audioClip = GetClip(EAudioType.Bgm, clip.ToString());
            _cachedBgmClips.Add((int)clip, audioClip);
        }

        if (_bgmAudioObject.gameObject.activeSelf && _bgmAudioObject.Clip == audioClip)
        {
            return;
        }
        
        _bgmAudioObject.gameObject.SetActive(true);
        _bgmAudioObject.PlayBgmAudio(clip, audioClip);
    }
    
    public void StopLoopSfxAudio(int audioObjectID)
    {
        foreach (AudioObject currentAudioObject in _allocatedSfxAudioObjects)
        {
            if (!currentAudioObject.gameObject.activeSelf
                || currentAudioObject.LoopSfxAudioObjectID != audioObjectID)
            {
                continue;
            }

            currentAudioObject.StopAudio();
            break;
        }
    }

    public void StopBgmAudio()
    {
        if (_bgmAudioObject.gameObject.activeSelf)
        {
            _bgmAudioObject.StopAudio();
        }
    }

    public void PauseAllAudio()
    {
        foreach (AudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf)
            {
                continue;
            }
            
            audioObject.PauseAudio();
        }
        
        _bgmAudioObject.PauseAudio();
    }

    public void UnPauseAllAudio()
    {
        foreach (AudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf)
            {
                continue;
            }
            
            audioObject.UnPauseAudio();
        }
        
        _bgmAudioObject.UnPauseAudio();
    }
}
