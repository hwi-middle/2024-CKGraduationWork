using System.Collections.Generic;
using UnityEngine;

public class RespawnData
{
    public class RespawnObjectData
    {
        public Vector3 startPosition;
        public bool isAlive;

        public RespawnObjectData(Vector3 pos)
        {
            startPosition = pos;
            isAlive = true;
        }
    }
    private static RespawnData _instance;
    private RespawnData() { }

    public static RespawnData Instance => _instance ??= new RespawnData();

    private Transform _enemiesRoot;
    private Transform _itemsRoot;

    private Vector3 _lastCheckPoint = new();
    private Dictionary<GameObject, RespawnObjectData> _initialEnemies = new();
    private Dictionary<GameObject, RespawnObjectData> _initialItems = new();

    private Dictionary<GameObject, RespawnObjectData> _savedEnemies = new();
    private Dictionary<GameObject, RespawnObjectData> _savedItems = new();

    public void InitData()
    {
        _enemiesRoot = RespawnHelper.Instance.Enemies;
        _itemsRoot = RespawnHelper.Instance.Items;
    
        _lastCheckPoint = PlayerMove.Instance.transform.position;
        for (int i = 0; i < RespawnHelper.Instance.Enemies.childCount; i++)
        {
            Transform enemy = _enemiesRoot.GetChild(i);
            RespawnObjectData objectData = new RespawnObjectData(enemy.position);
            _initialEnemies.Add(enemy.gameObject, objectData);
        }

        for (int i = 0; i < RespawnHelper.Instance.Items.childCount; i++)
        {
            Transform item = _itemsRoot.GetChild(i);
            RespawnObjectData objectData = new RespawnObjectData(item.position);
            _initialItems.Add(item.gameObject, objectData);
        }
    }

    public void SaveCheckPoint(Vector3 checkPosition)
    {
        _lastCheckPoint = checkPosition;
        for (int i = 0; i < _enemiesRoot.childCount; i++)
        {
            GameObject enemy = _enemiesRoot.GetChild(i).gameObject;
            RespawnObjectData objectData = _initialEnemies[enemy.gameObject];
            _savedEnemies.Add(enemy, objectData);
        }

        for (int i = 0; i < _itemsRoot.childCount; i++)
        {
            
        }
    }
}