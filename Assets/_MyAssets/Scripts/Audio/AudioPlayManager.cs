using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum EAudioType
{
    Bgm,
    Sfx
}

public enum ESfxPlayType
{
    PlayOnce,
    Loop
}

public class AudioPlayManager : Singleton<AudioPlayManager>
{
    [SerializeField] private AudioMixer _mainMixer;
    [SerializeField] private AudioClipData _audioClipData;

    private AudioMixerGroup _bgmMixerGroup;
    private AudioMixerGroup _sfxMixerGroup;
    
    // 배경음 오브젝트 풀
    private BgmAudioObject _bgmAudioObject;
    // 효과음 오브젝트 풀
    private readonly List<SfxAudioObject> _allocatedSfxAudioObjects = new();
    
    // string 할당을 단 한번만 하기 위한 Dictionary
    private readonly Dictionary<EBgmAudioClipIndex, AudioClip> _cachedBgmClips = new();
    private readonly Dictionary<ESfxAudioClipIndex, AudioClip> _cachedSfxClips = new();
    
    // 루프 중인 효과음 오브젝트를 관리하기 위한 Dictionary
    private readonly Dictionary<ESfxAudioClipIndex, int> _loopSfxAudioObjects = new();
    
    private void Awake()
    {
        InitMixerGroup();
        InstantiateBgmAudioObject();
        InstantiateSfxAudioObject();
    }

    private void InitMixerGroup()
    {
        _bgmMixerGroup = _mainMixer.FindMatchingGroups("Master")[1];
        _sfxMixerGroup = _mainMixer.FindMatchingGroups("Master")[2];
    }

    private void InstantiateBgmAudioObject()
    {
        GameObject bgmObject = new();
        bgmObject.AddComponent<AudioSource>();
        bgmObject.AddComponent<BgmAudioObject>();
        bgmObject.name = "BgmAudioObject";
        bgmObject.transform.SetParent(transform);
        bgmObject.GetComponent<AudioSource>().outputAudioMixerGroup = _bgmMixerGroup;
        bgmObject.SetActive(false);
        
        _bgmAudioObject = bgmObject.GetComponent<BgmAudioObject>();
    }

    private void InstantiateSfxAudioObject()
    {
        GameObject sfxObject = new();
        sfxObject.AddComponent<AudioSource>();
        sfxObject.AddComponent<SfxAudioObject>();

        for (int i = 0; i < _audioClipData.audioObjectMaxCapacity; i++)
        {
            GameObject sfxObjectClone = Instantiate(sfxObject, transform);
            _allocatedSfxAudioObjects.Add(sfxObjectClone.GetComponent<SfxAudioObject>());
            sfxObjectClone.name = "SfxAudioObject_" + i;
            sfxObjectClone.GetComponent<AudioSource>().outputAudioMixerGroup = _sfxMixerGroup;
            sfxObjectClone.SetActive(false);
        }
        
        Destroy(sfxObject);
    }

    private SfxAudioObject GetAvailableAudioObject()
    {
        foreach(SfxAudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf)
            {
                return audioObject;
            }
        }

        Debug.Assert(false, "Invalid Situation : Audio Object Pool is full.");
        return null;
    }
    
    private AudioClip GetClip(EAudioType audioType, string clipName)
    {
        List<AudioClipInfo> clipInfoList = null;

        switch (audioType)
        {
            case EAudioType.Bgm:
                clipInfoList = _audioClipData.bgmClipList;
                break;
            case EAudioType.Sfx:
                clipInfoList = _audioClipData.sfxClipList;
                break;
            default:
                Debug.Assert(false);
                break;
        }
        
        foreach(AudioClipInfo clipInfo in clipInfoList)
        {
            if (clipName.Equals(clipInfo.clipName))
            {
                return clipInfo.clip;
            }
        }

        Debug.Assert(false, "Invalid Situation : Clip is not found.");
        return null;
    }

    public void PlayOnceSfxAudio(ESfxAudioClipIndex clip)
    {
        if (!_cachedSfxClips.TryGetValue(clip, out AudioClip audioClip))
        {
            audioClip = GetClip(EAudioType.Sfx, clip.ToString());
            _cachedSfxClips.Add(clip, audioClip);
        }

        SfxAudioObject availableObject = GetAvailableAudioObject();

        availableObject.gameObject.SetActive(true);
        availableObject.Play(audioClip, ESfxPlayType.PlayOnce);
    }

    public void PlayLoopSfxAudio(ESfxAudioClipIndex clip, Transform parent = null)
    {
        if(!_cachedSfxClips.TryGetValue(clip, out AudioClip audioClip))
        {
            audioClip = GetClip(EAudioType.Sfx, clip.ToString());
            _cachedSfxClips.Add(clip, audioClip);
        }

        Debug.Assert(audioClip.name != null);

        // 중복 재생 방지
        int playingLoopSfxID = CheckIsPlayingLoopSfx(audioClip);
        if (playingLoopSfxID != 0)
        {
            return;
        }
        
        // 사용 가능한 오브젝트를 찾음
        SfxAudioObject availableObject = GetAvailableAudioObject();
        
        availableObject.gameObject.SetActive(true);
        availableObject.Play(audioClip, ESfxPlayType.Loop);
        
        // 루프 중인 효과음 오브젝트를 관리하기 위한 Dictionary에 추가
        _loopSfxAudioObjects.Add(clip, availableObject.GetInstanceID());
        if (parent is null)
        {
            return;
        }
        
        availableObject.transform.SetParent(parent);
    } 
    
    private int CheckIsPlayingLoopSfx(AudioClip audioClip)
    {
        foreach (SfxAudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf || !audioObject.IsLoop)
            {
                continue;
            }
            
            if (audioObject.Clip.name == audioClip.name)
            {
                return audioObject.GetInstanceID();
            }
        }

        return 0;
    }

    public void PlayBgmAudio(EBgmAudioClipIndex clip)
    {
        if (!_cachedBgmClips.TryGetValue(clip, out AudioClip audioClip))
        {
            audioClip = GetClip(EAudioType.Bgm, clip.ToString());
            _cachedBgmClips.Add(clip, audioClip);
        }

        if (_bgmAudioObject.gameObject.activeSelf && _bgmAudioObject.Clip == audioClip)
        {
            return;
        }
        
        _bgmAudioObject.gameObject.SetActive(true);
        _bgmAudioObject.Play(audioClip);
    }
    
    public void StopLoopSfxAudio(ESfxAudioClipIndex clip)
    {
        if(_loopSfxAudioObjects.Count == 0 || !_loopSfxAudioObjects.TryGetValue(clip, out int id))
        {
            return;
        }
        
        foreach (SfxAudioObject currentAudioObject in _allocatedSfxAudioObjects)
        {
            if (!currentAudioObject.gameObject.activeSelf
                || currentAudioObject.GetInstanceID() != id) 
            {
                continue;
            }

            _loopSfxAudioObjects.Remove(clip);
            currentAudioObject.Stop();
            if (currentAudioObject.transform.parent != transform)
            {
                currentAudioObject.transform.SetParent(transform);
            }
            
            break;
        }
    }

    public void StopBgmAudio()
    {
        if (_bgmAudioObject.gameObject.activeSelf)
        {
            _bgmAudioObject.Stop();
        }
    }

    public void PauseAllAudio()
    {
        foreach (SfxAudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf)
            {
                continue;
            }
            
            audioObject.Pause();
        }
        
        _bgmAudioObject.Pause();
    }

    public void UnPauseAllAudio()
    {
        foreach (SfxAudioObject audioObject in _allocatedSfxAudioObjects)
        {
            if (!audioObject.gameObject.activeSelf)
            {
                continue;
            }
            
            audioObject.UnPause();
        }
        
        _bgmAudioObject.UnPause();
    }
}
