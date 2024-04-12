using System;
using System.Collections;
using System.Collections.Generic;
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
        
    }
    
    public int TakeDamage(int damageAmount, GameObject damageCauser)
    {
        Debug.Log("Player TakeDamage()");
        _hp -= damageAmount;
        return damageAmount;
    }
}
