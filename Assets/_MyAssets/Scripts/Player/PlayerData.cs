using UnityEngine;

namespace _MyAssets.Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [Header("플레이어 이동 속도")]
        public float moveSpeed;
        
        [Header("플레이어 점프 높이")]
        public float jumpHeight;
        
        [Header("플레이어 경사로 미끄러짐 속도")]
        public float slideSpeed;

        [Header("플레이어 공격 사거리")] 
        public float attackRange;
        
        [Header("플레이어 공격 대미지")]
        public float attackDamage;
    }
}
