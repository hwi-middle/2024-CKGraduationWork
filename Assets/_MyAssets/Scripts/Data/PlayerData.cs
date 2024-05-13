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

    [Tooltip("앉기 속도")] public float crouchSpeed;
    
    [Tooltip("플레이어 회전 속도")] public float rotateSpeed;

    [Tooltip("점프 높이")] public float jumpHeight;
    [Tooltip("y 이동 속도 (일반 이동 속도와 분리하기 위함)")] public float yMultiplier;

    [Header("장애물 극복 액션")]
    [Tooltip("장애물 극복 시간(초)")] public float overstepActionDuration;

    [Tooltip("장애물 넘기 도착 거리")] public float overstepTargetDistance;

    [Header("암살 액션")]
    [Tooltip("암살 소요 시간(초)")] public float assassinateDuration;

    [Header("조준 시")]
    [Tooltip("조준 시 이동 속도")] public float moveSpeedOnAim;

    [Tooltip("아이템 던지는 힘")] public float throwPower;
    [Tooltip("아이템 최대 사거리")] public float maxItemRange;
    [Tooltip("아이템 영향 거리 (반지름)")] public float itemImpactRadius;
    [Tooltip("아이템 적 게이지 증가량")] public float itemGaugeAmount;

    [Header("숨기")]
    [Tooltip("숨을 수 있는 오브젝트 최대 거리")] public float maxDistanceHideableObject;
    
    [Header("플레이어가 내는 소리")]
    [Tooltip("걷기 시 소리 반경")] public float walkNoiseRadius;
    [Tooltip("걷기 시 초당 인지게이지 증가량")] public float walkNoiseIncrementPerSecond;
    [Tooltip("달리기 시 소리 반경")] public float sprintNoiseRadius;
    [Tooltip("달리기 시 초당 인지게이지 증가량")] public float sprintNoiseIncrementPerSecond;
    [Tooltip("앉은 상태로 걷기 시 소리 반경")] public float crouchWalkNoiseRadius;
    [Tooltip("앉은 상태로 걷기 시 초당 인지게이지 증가량")] public float crouchWalkNoiseIncrementPerSecond;
    
    [Header("-----변경 시 주의-----")]
    [Header("와이어 및 플레이어 Canvas")]
    [Tooltip("Player Canvas Prefab")] public GameObject playerCanvas;

    [Tooltip("Line Renderer")] public GameObject lineRendererPrefab;

    [Header("카메라 오브젝트")]
    [Tooltip("플레이어를 따라다니는 카메라 프리팹")] public GameObject camerasPrefab;
}