using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionBound : MonoBehaviour
{
    public bool IsOnSight { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        IsOnSight = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IsOnSight = false;
    }
}
