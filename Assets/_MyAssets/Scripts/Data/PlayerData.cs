using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Scriptable Object Asset/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("플레이어 체력")]
    [Tooltip("정수형 플레이어 체력")] public int playerHp;

    [Header("플레이어 이동")]
    [Tooltip("걷기 속도")] public float walkSpeed;

    [Tooltip("달리기 속도")] public float runSpeed;

    [Tooltip("점프 높이")] public float jumpHeight;
    [Tooltip("y 이동 속도 (일반 이동 속도와 분리하기 위함)")] public float yMultiplier;

    [Tooltip("앉기 속도")] public float crouchSpeed;

    [Tooltip("벽 이동 속도")] public float wallMoveSpeed;

    [Header("경사로 미끄러짐 속도")]
    [Tooltip("올라기지 못하는 경사로에서 미끄러질 때 속도")] public float slopeSlideSpeed;

    [Header("와이어 및 플레이어 Canvas")]
    [Tooltip("Player Canvas Prefab")] public GameObject playerCanvas;
}