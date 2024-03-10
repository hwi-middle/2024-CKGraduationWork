using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("몬스터 이동 속도")]
    public float moveSpeed;
    
    [Header("몬스터 점프 높이")]
    public float jumpHeight;
    
    [Header("몬스터 공격 대미지")]
    public float attackDamage;
    
    [Header("(?)몬스터 감지 거리")]
    public float detectRange;
    
    [Header("몬스터 공격 범위")]
    public float attackRange;
}
