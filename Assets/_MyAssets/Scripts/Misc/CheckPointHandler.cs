using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointHandler : MonoBehaviour
{
    public static int checkPointCount = 0;
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

        checkPointCount++;
        RespawnHelper.Instance.SaveCheckPoint(transform.position);
        GetComponent<Collider>().enabled = false;
    }
}
