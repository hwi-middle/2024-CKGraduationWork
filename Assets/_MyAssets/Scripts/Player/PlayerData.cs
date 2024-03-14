using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("---플레이어 이동 변수---")]
    
    [Header("체력")] public int hp;
    
    [Header("걷기 속도")] public float walkSpeed;

    [Header("달리기 속도")] public float runSpeed;

    [Header("앉기 이동 속도")] public float crouchSpeed;

    [Header("점프 높이")] public float jumpHeight;
    

    [Header("---플레이어 공격 관련 변수---")]

    [Header("공격 사거리")] public float attackRange;

    [Header("공격 대미지")] public float attackDamage;

    [Header("방어 경직 시간")] public float guardStunTime;

    [Header("방어 가능 각도")] public float guardAvailableDegree;
    
    
    [Header("---특수 이동 관련 변수---")]
    
    [Header("와이어 최대 이동 거리")] public float maxWireRange;

    [Header("와이어 이동 시간")] public float wireMoveTime;

    [Header("비집기 이동 속도")] public float wallMoveSpeed;

    [Header("포지션 이동 거리")] public float pMoveRange;

    [Header("포지션 이동 속도")] public float pMoveSpeed;
    
    
    [Header("---프로그래머 관리 영역---")]
    
    [Header("무기 프리팹 : 가위")] public GameObject scissor;
    
    [Header("플레이어 경사로 미끄러짐 속도")] public float slideSpeed;
    
}
