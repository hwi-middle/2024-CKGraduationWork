using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SoundClip Data", menuName = "Scriptable Object Asset/Sound Clip Data")]
public class SoundClipData : ScriptableObject
{
    public int maxSoundObjectCount = 10;
    public List<AudioClip> bgmClipList = new();
    public List<AudioClip> sfxClipList = new();
}
