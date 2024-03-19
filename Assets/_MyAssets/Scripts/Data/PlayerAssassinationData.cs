using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewPlayerAssassinationData", menuName = "Scriptable Object Asset/Player Assassination Data")]
public class PlayerAssassinationData : ScriptableObject
{
    [Header("암살 소요 시간")]
    [Tooltip("낙하(위->아래) 암살 시간")] public float fallAssassinationDuration;
    [Tooltip("점프(아래->위) 암살 시간")] public float jumpAssassinationDuration;
    [Tooltip("평지 암살 시간")] public float groundAssassinationDuration;
    
    [Header("암살 판정 관련 값")]
    [Tooltip("낙하(위->아래) 암살 판정 임계값")] public float fallAssassinationHeightThreshold;
    [Tooltip("점프(아래->위) 암살 판정 임계값")] public float jumpAssassinationHeightThreshold;
    [Tooltip("암살 가능 거리")] public float assassinateDistance;
}