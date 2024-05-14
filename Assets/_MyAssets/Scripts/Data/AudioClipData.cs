using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AudioClipInfo
{
    public string clipName;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "New AudioClip Data", menuName = "Scriptable Object Asset/Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    public int maxAudioObjectCount = 20;
    public List<AudioClipInfo> bgmClipList = new();
    public List<AudioClipInfo> sfxClipList = new();
}
