using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Throw,
    Use
}

[Serializable]
public class Item
{
    public ItemType type;
    public string name;
    public GameObject item;
}

[CreateAssetMenu(fileName = "New Player Item", menuName = "Scriptable Object Asset/Player Item Data")]
public class PlayerItemData : ScriptableObject
{
    public List<Item> items;
}
