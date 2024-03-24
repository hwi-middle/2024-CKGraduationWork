using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWireData", menuName = "Scriptable Object Asset/Player Wire Data")]
public class PlayerWireData : ScriptableObject
{
    [Header("와이어 최소 거리")] public float minWireDistance;
    [Header("와이어 최대 거리")] public float maxWireDistance;
    [Header("와이어 액션 시간(초)")] public float wireActionDuration;
}