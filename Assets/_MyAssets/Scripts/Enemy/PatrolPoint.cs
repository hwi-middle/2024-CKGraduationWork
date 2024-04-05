using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    private Vector3 _originalPos;
    private Quaternion _originalRot;

    private void Awake()
    {
        _originalPos = transform.position;
        _originalRot = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.position = _originalPos;
        transform.rotation = _originalRot;
    }
}
