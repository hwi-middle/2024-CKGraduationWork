using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHelper : Singleton<RespawnHelper>
{
    [SerializeField] private Transform _enemies;
    [SerializeField] private Transform _items;

    public Transform Enemies => _enemies;
    public Transform Items => _items;
    
    private void Start()
    {
        
    }

    public void SaveCheckPoint(Vector3 checkPosition)
    {
        RespawnData.Instance.SaveCheckPoint(checkPosition);
    }
}
