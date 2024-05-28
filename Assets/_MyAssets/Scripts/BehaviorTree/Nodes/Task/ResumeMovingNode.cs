using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeMovingNode : TaskNode
{
    public override void OnCreate()
    {
        description = "중지된 길 찾기를 재개합니다.";
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
        agent.NavMeshAgent.isStopped = false;
        
        return ENodeState.Success;
    }
}
