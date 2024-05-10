using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New AudioClip Data", menuName = "Scriptable Object Asset/Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    public int maxAudioObjectCount = 10;
    public List<AudioClip> bgmClipList = new();
    public List<AudioClip> sfxClipList = new();
}
