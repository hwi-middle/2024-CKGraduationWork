using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAiData", menuName = "Scriptable Object Asset/Enemy AI Data")]
public class EnemyAiData : ScriptableObject
{
    [Header("플레이어 인지")]
    [Tooltip("인식 거리 별 게이지 증가량 그래프")] public AnimationCurve perceptionGaugeCurve;
    
    [Tooltip("초당 인지 게이지 증가량(최대)")] public float maxPerceptionGaugeIncrementPerSecond;
    [Tooltip("초당 게이지 감소량")] public float gaugeDecrementPerSecond;
    [Tooltip("경계 돌입 기준값")] public float alertThreshold;
    [Tooltip("청각으로 인한 게이지 증가 최댓값")] public float maxPerceptionGaugeByHearing;
    
    [Header("이동 관련")]
    [Tooltip("걷기 이동 속도")] public float walkSpeed;
    [Tooltip("달리기 이동 속도")] public float sprintSpeed;
    [Tooltip("활동 범위(반경)")] public float moveRange;
    [Tooltip("정지 거리")] public float stoppingDistance;

    [Header("공격 관련")]
    [Tooltip("공격력")] public int attackDamage;
    [Tooltip("공격 범위(반경)")] public float attackRange;
    [Tooltip("공격 딜레이(초)")] public float attackDelay;
}