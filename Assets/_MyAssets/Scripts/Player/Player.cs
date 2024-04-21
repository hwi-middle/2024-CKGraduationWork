using UnityEngine;

public class Player : Singleton<Player>, IDamageable
{
    [SerializeField] private PlayerData _playerData;
    
    public PlayerData PlayerData => _playerData;
    
    private int _hp;
    public int Hp => _hp;
    

    private void Awake()
    {
        _hp = _playerData.playerHp;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (SceneManagerBase.Instance.IsDebugMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                _hp = 0;
            }
        }
    }

    public bool IsPlayerDead()
    {
        return _hp <= 0;
    }
    
    public int TakeDamage(int damageAmount, GameObject damageCauser)
    {
        Debug.Log("Player TakeDamage()");
        _hp -= damageAmount;
        return damageAmount;
    }
}
