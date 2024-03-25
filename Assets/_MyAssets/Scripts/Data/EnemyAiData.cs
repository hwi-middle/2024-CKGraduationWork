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
    [Tooltip("거리(%)별 초당 게이지 증가량")] public List<EnemyAiPerceptionRange> perceptionRanges;
    [Tooltip("게이지 감소 시작 시간")] public float gaugeDecreaseStartTime;
    [Tooltip("초당 게이지 감소량")] public float gaugeDecrementPerSecond;
    
    [Header("능력치")]
    [Tooltip("이동 속도")] public float moveSpeed;
    [Tooltip("활동 범위(반경)")] public float moveRange;
}

[Serializable]
public struct EnemyAiPerceptionRange
{
    [Tooltip("거리(%)"), Range(0, 100)] public float rangePercent;
    [Tooltip("초당 게이지 증가량")] public float gaugeIncrementPerSecond;
}
