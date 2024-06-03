using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMovingNode : TaskNode
{
    public override void OnCreate()
    {
        description = "길 찾기를 중지하고 멈춥니다.";
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
        agent.NavMeshAgent.isStopped = true;
        
        return ENodeState.Success;
    }
}
