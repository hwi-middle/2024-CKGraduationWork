using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractionData", menuName = "Scriptable Object Asset/Interaction Data")]
public class InteractionData : ScriptableObject
{
    [Header("거리")]
    [Tooltip("플레이어를 감지하는 거리")] public float detectRadius;

    [Header("오브젝트 유형")]
    [Tooltip("아이템인지 숨기 가능한 오브젝트 인지")] public EInteractionType type;
}
