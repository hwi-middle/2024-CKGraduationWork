using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointHandler : MonoBehaviour
{
    public static int currentCheckPointCount = 0;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = CheckPointSceneManager.Instance.IsDebugMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }

        currentCheckPointCount++;
        Debug.Log(currentCheckPointCount);
        GetComponent<Collider>().enabled = false;
    }
}
