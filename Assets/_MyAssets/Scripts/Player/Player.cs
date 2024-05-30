using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>, IDamageable
{
    [SerializeField] private PlayerData _playerData;
    private readonly List<ESfxAudioClipIndex> _playerHitSounds = new();
    
    public PlayerData PlayerData => _playerData;
    
    private int _hp;
    public int Hp => _hp;
    

    private void Awake()
    {
        _hp = _playerData.playerHp;
        InitHitSounds();
    }

    private void InitHitSounds()
    {
        _playerHitSounds.Add(ESfxAudioClipIndex.Player_Hit1);
        _playerHitSounds.Add(ESfxAudioClipIndex.Player_Hit2);
        _playerHitSounds.Add(ESfxAudioClipIndex.Player_Hit3);
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
        AudioPlayManager.Instance.PlayOnceSfxAudio(_playerHitSounds[Random.Range(0, _playerHitSounds.Count)]);
        return damageAmount;
    }
}
