using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewPlayerAssassinationData", menuName = "Scriptable Object Asset/Player Assassination Data")]
public class PlayerAssassinationData : ScriptableObject
{
    [Header("암살 소요 시간")]
    [Tooltip("공중 암살 시간")] public float jumpAssassinationDuration;
    [Tooltip("평지 암살 시간")] public float groundAssassinationDuration;
    
    [Header("암살 구분 기준")]
    [Tooltip("공중 암살 판정 임계값")] public float jumpAssassinationHeightThreshold;
}
