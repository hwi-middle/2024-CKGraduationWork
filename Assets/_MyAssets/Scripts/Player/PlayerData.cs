using UnityEngine;

namespace _MyAssets.Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public float moveSpeed;
        public float jumpHeight;
        public float slideSpeed;
        public float attackDamage;
    }
}
