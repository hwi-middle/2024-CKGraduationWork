using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SightBound : MonoBehaviour
{
    public bool IsOnSight { get; private set; }

    protected virtual void Awake()
    {
        GetComponent<Renderer>().enabled = SceneManagerBase.Instance.IsDebugMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        IsOnSight = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IsOnSight = false;
    }
}
