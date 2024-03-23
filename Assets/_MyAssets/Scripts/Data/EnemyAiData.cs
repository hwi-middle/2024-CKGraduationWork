using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewEnemyAiData", menuName = "Scriptable Object Asset/Enemy AI Data")]
public class EnemyAiData : ScriptableObject
{
    [Header("플레이어 인지")]
    [Tooltip("인식 거리(반경)"), Range(0.5f, 100)] public float perceptionDistance;
    [Tooltip("인식 범위(각도)"), Range(0, 360)] public float perceptionAngle;
    public List<EnemyAiPerceptionRange> perceptionRanges;
    
    [Header("능력치")]
    [Tooltip("이동 속도")] public float moveSpeed;
}

[Serializable]
public struct EnemyAiPerceptionRange
{
    [Range(0, 100)] public float rangeRatio;
    public float gaugeIncrementPerSecond;
}
