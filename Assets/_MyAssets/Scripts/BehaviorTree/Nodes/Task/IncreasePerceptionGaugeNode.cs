using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasePerceptionGaugeNode : TaskNode
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
        var player = PlayerMove.Instance;
        if (player == null)
        {
            return ENodeState.Failure;
        } 
        
        float distance= Vector3.Distance(agent.transform.position, player.transform.position);
        agent.IncrementPerceptionGauge(distance);
        
        return ENodeState.Success;
    }
}
