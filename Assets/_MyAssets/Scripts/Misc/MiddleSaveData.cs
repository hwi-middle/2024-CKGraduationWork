using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleSaveData : Singleton<MiddleSaveData>
{
    private class ObjectSaveData
    {
        public readonly Vector3 position;
        public bool isAlive;

        public ObjectSaveData(Vector3 position)
        {
            this.position = position;
            isAlive = true;
        }
    }
    
    // Enemy와 Item은 각 Empty Object로 Root를 가지고 있어야 함
    [SerializeField] private Transform _enemyRoot;
    [SerializeField] private Transform _itemRoot;
    
    // Load를 위해 저장 된 Data
    private Dictionary<GameObject, ObjectSaveData> _savedEnemies = new();
    private Dictionary<GameObject, ObjectSaveData> _savedItems = new();

    // Save를 위해 저장 된 Data
    private Dictionary<GameObject, ObjectSaveData> _changedEnemies = new();
    private Dictionary<GameObject, ObjectSaveData> _changedItems = new();

    private void Awake()
    {
        InitEnemies();
        InitItems();
    }

    public void KillEnemy(GameObject enemy)
    {
        _changedEnemies[enemy].isAlive = false;
    }

    public void UseItem(GameObject item)
    {
        _changedItems[item].isAlive = false;
    }

    public void MiddleSave()
    {
        SaveEnemies();
        SaveItems();
    }

    public void LoadSavedData()
    {
        LoadEnemies();
        LoadItems();
    }

    private void InitEnemies()
    {
        int enemyCount = _enemyRoot.childCount;
        for (int i = 0; i < enemyCount; i++)
        {
            Transform enemy = _enemyRoot.GetChild(i);
            ObjectSaveData objectSaveData = new(enemy.position);
            _savedEnemies.Add(enemy.gameObject, objectSaveData);   
        }

        _changedEnemies = new(_savedEnemies);
    }

    private void InitItems()
    {
        int itemCount = _itemRoot.childCount;
        for (int i = 0; i < itemCount; i++)
        {
            Transform item = _itemRoot.GetChild(i);
            ObjectSaveData itemSaveData = new(item.position);
            _savedItems.Add(item.gameObject, itemSaveData);
        }

        _changedItems = new(_savedItems);
    }

    private void SaveEnemies()
    {
        _savedEnemies.Clear();
        int remainEnemyCount = _enemyRoot.childCount;
        for (int i = 0; i < remainEnemyCount; i++)
        {
            Transform enemy = _enemyRoot.GetChild(i);
            ObjectSaveData objectSaveData = _changedEnemies[enemy.gameObject];
            
            if (!objectSaveData.isAlive)
            {
                _changedEnemies.Remove(enemy.gameObject);
                Destroy(enemy.gameObject);
                continue;
            }

            _savedEnemies.Add(enemy.gameObject, objectSaveData);
        }

        _changedEnemies.Clear();
        _changedEnemies = new(_savedEnemies);
    }

    private void SaveItems()
    {
        _savedItems.Clear();
        int remainItemCount = _itemRoot.childCount;
        for (int i = 0; i < remainItemCount; i++)
        {
            Transform item = _itemRoot.GetChild(i);

            if (_changedItems[item.gameObject].isAlive)
            {
                _changedItems.Remove(item.gameObject);
                Destroy(item.gameObject);
                continue;
            }
            
            ObjectSaveData itemSaveData = new ObjectSaveData(item.position);
            _savedItems.Add(item.gameObject, itemSaveData);
        }

        _changedItems = new(_savedItems);
    }

    private void LoadEnemies()
    {
        foreach (var enemy in _savedEnemies)
        {
            enemy.Key.SetActive(true);
            enemy.Value.isAlive = true;
            enemy.Key.transform.position = enemy.Value.position;
        }
    }

    private void LoadItems()
    {
        foreach (var item in _savedItems)
        {
            item.Key.SetActive(true);
            item.Value.isAlive = true;
            item.Key.transform.position = item.Value.position;
        }
    }
}
