using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode
{
    public float duration = 1f;
    private float startTime;

    public override void OnCreate()
    {
        description = "지정한 시간동안 대기합니다.";
    }
    
    protected override void OnStart()
    {
        startTime = Time.time;
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
