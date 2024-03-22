using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerBaseData", menuName = "Scriptable Object Asset/Player Base Data")]
public class PlayerBaseData : ScriptableObject
{
    [Header("플레이어 체력")]
    [Tooltip("정수형 플레이어 체력")] public int playerHp;
    
    [Header("플레이어 이동")]
    [Tooltip("걷기 속도")] public float walkSpeed;

    [Tooltip("달리기 속도")] public float runSpeed;
    
    [Tooltip("점프 높이")] public float jumpHeight;
    [Tooltip("점프 속도(걷기 속도와 비슷하거나 동일)")] public float jumpSpeed;
    
    [Tooltip("앉기 속도")] public float crouchSpeed;

    [Tooltip("벽 이동 속도")] public float wallMoveSpeed;

    [Header("경사로 미끄러짐 속도")]
    [Tooltip("올라기지 못하는 경사로에서 미끄러질 때 속도")] public float slopeSlideSpeed;
}
