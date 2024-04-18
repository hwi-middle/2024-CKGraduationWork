using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleSaveData : Singleton<MiddleSaveData>
{
    private class ObjectSaveData
    {
        public GameObject obj;
        public readonly Vector3 position;
        public bool isAlive;

        public ObjectSaveData(GameObject obj)
        {
            this.obj = obj;
            position = obj.transform.position;
            isAlive = true;
        }
    }

    // Enemy와 Item은 각 Empty Object로 Root를 가지고 있어야 함
    [SerializeField] private Transform _enemyRoot;
    [SerializeField] private Transform _itemRoot;

    // Load를 위해 저장 된 Data
    private Dictionary<int, ObjectSaveData> _savedEnemies = new();
    private Dictionary<int, ObjectSaveData> _savedItems = new();

    // Save를 위해 저장 된 Data
    private Dictionary<int, ObjectSaveData> _changedEnemies = new();
    private Dictionary<int, ObjectSaveData> _changedItems = new();

    private void Awake()
    {
        InitEnemies();
        InitItems();
    }

    public void KillEnemy(int key)
    {
        Debug.Log(_changedEnemies[key].obj.name);
        _changedEnemies[key].isAlive = false;
    }

    public void UseItem(int key)
    {
        _changedItems[key].isAlive = false;
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
            int key = enemy.GetInstanceID();
            Debug.Log(enemy.GetInstanceID());
            ObjectSaveData objectSaveData = new(enemy.gameObject);
            _savedEnemies.Add(key, objectSaveData);   
        }

        _changedEnemies = new(_savedEnemies);
    }

    private void InitItems()
    {
        int itemCount = _itemRoot.childCount;
        for (int i = 0; i < itemCount; i++)
        {
            Transform item = _itemRoot.GetChild(i);
            int key = item.GetInstanceID();
            ObjectSaveData itemSaveData = new(item.gameObject);
            _savedItems.Add(key, itemSaveData);
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
            int key = enemy.GetInstanceID();
            ObjectSaveData objectSaveData = _changedEnemies[key];
            
            if (!objectSaveData.isAlive && !enemy.gameObject.activeSelf)
            {
                Debug.Log($"Enemy is Dead {enemy.gameObject.name}");
                _changedEnemies.Remove(key);
                Destroy(enemy.gameObject);
                continue;
            }

            _savedEnemies.Add(key, objectSaveData);
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
            int key = item.GetInstanceID();
            ObjectSaveData objectSaveData = _changedItems[key];

            if (!objectSaveData.isAlive)
            {
                _changedItems.Remove(key);
                Destroy(item.gameObject);
                continue;
            }
            
            _savedItems.Add(key, objectSaveData);
        }

        _changedItems.Clear();
        _changedItems = new(_savedItems);
    }

    private void LoadEnemies()
    {
        foreach (var enemy in _savedEnemies)
        {
            enemy.Value.obj.SetActive(true);
            enemy.Value.isAlive = true;
            enemy.Value.obj.transform.position = enemy.Value.position;
        }

        _changedEnemies = new(_savedEnemies);
    }

    private void LoadItems()
    {
        foreach (var item in _savedItems)
        {
            item.Value.obj.SetActive(true);
            item.Value.isAlive = true;
            item.Value.obj.transform.position = item.Value.position;
        }

        _changedItems = new(_savedItems);
    }
}
