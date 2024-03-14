using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewPlayerAssassinationData", menuName = "Scriptable Object Asset/Player Assassination Data")]
public class PlayerAssassinationData : ScriptableObject
{
    [Tooltip("암살 시간")] public float jumpAssassinationDuration;
}
