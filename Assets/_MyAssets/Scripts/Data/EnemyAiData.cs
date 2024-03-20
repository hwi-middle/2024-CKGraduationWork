using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewEnemyAiData", menuName = "Scriptable Object Asset/Enemy AI Data")]
public class EnemyAiData : ScriptableObject
{
    [Header("플레이어 인지")]
    [Tooltip("인식 거리(반경)")] public float perceptionDistance;
    [Tooltip("인식 범위(각도)")] public float perceptionAngle;
}
