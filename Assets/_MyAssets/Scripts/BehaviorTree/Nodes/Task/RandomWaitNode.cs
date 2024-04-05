using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWaitNode : TaskNode
{
    public float durationMin = 1f;
    public float durationMax = 2f;
    private float duration = 0f;
    private float startTime;

    public override void OnCreate()
    {
        description = "지정한 시간 범위 내에서 랜덤으로 대기합니다.";
    }
    
    protected override void OnStart()
    {
        startTime = Time.time;
        duration = Random.Range(durationMin, durationMax);
    }

    protected override void OnStop()
    {
        
    }

    protected override void OnAbort()
    {
        
    }

    protected override ENodeState OnUpdate()
    {
        if (Time.time - startTime > duration)
        {
            return ENodeState.Success;
        }

        return ENodeState.Running;
    }
}
