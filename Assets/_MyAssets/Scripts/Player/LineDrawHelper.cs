using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawHelper : Singleton<LineDrawHelper>
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
        
        // 그라데이션 설정
        Gradient gradient = new();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        
        _line.colorGradient = gradient;
    }

    public void EnableLine()
    {
        _line.enabled = true;
    }

    public void DisableLine()
    {
        _line.enabled = false;
    }

    public void DrawWire(Vector3 startPosition, Vector3 targetPosition)
    {
        _line.positionCount = 2;
        startPosition.y += 0.5f;
        _line.SetPosition(0, startPosition);
        _line.SetPosition(1, targetPosition);
    }

    public void SetPositionCount(int count)
    {
        _line.positionCount = count;
    }

    public void DrawParabola(Vector3[] list)
    {
        _line.SetPositions(list);
    }
}