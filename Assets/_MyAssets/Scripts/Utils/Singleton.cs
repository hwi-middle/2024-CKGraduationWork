using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Init();
            }

            return _instance;
        }
    }
    
    private static void Init()
    {
        T instance = FindObjectOfType<T>();
        if (instance == null)
        {
            Debug.LogError($"{typeof(T)} not found");
        }

        _instance = instance.GetComponent<T>();
    }
}