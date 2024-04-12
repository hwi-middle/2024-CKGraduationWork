using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSamplePlayerData", menuName = "Scriptable Object Asset/Sample Player Data")]
public class SamplePlayerData : ScriptableObject
{
    [Header("능력치")]
    [Tooltip("플레이어의 체력입니다.")] public float hp;
    [Tooltip("플레이어의 마나입니다.")] public int mp;

}

