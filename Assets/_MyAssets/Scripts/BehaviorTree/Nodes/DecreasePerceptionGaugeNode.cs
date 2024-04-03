using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreasePerceptionGaugeNode : TaskNode
{
    public override void OnCreate()
    {
        description = "인지 게이지를 증가시킵니다.";
    }

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override void OnAbort()
    {
    }

    protected override ENodeState OnUpdate()
    {
        agent.DecrementPerceptionGauge();
        
        return ENodeState.Success;
    }
}
