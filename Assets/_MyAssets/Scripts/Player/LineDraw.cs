using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : Singleton<LineDraw>
{
    private LineRenderer _line;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _line.startWidth = _line.endWidth = 0.1f;
        _line.positionCount = 1;
    }

    public void TurnOnLine()
    {
        _line.enabled = true;
    }

    public void TurnOffLine()
    {
        _line.enabled = false;
    }

    public void Draw(Vector3 startPosition, Vector3 targetPosition)
    {
        _line.positionCount = 2;
        _line.SetPosition(0, startPosition);
        _line.SetPosition(1, targetPosition);
    }
}