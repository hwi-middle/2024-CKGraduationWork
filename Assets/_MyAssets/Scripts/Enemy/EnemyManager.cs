using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyData _myData;
    private IEnumerator _attack;
    
    public bool IsUnderAttack { get; private set; }
    
    public float AttackTime { get; private set; }

    private void Awake()
    {
        _attack = AttackRoutine();
        IsUnderAttack = false;
    }

    private void Update()
    {
        
    }

    public void Attack()
    {
        if (_attack == null)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        yield return null;
    }
}
