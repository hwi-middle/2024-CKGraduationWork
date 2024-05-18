using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class TutorialVideo
{
    public string title;
    public VideoClip videoClip;
}

[CreateAssetMenu(fileName = "New Tutorial Video", menuName = "Scriptable Object Asset/Tutorial Video")]
public class TutorialVideoData : ScriptableObject
{
    public List<TutorialVideo> tutorialVideos;
}
