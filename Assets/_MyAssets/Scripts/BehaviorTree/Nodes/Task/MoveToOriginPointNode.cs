using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToOriginPointNode : TaskNode
{
    public override void OnCreate()
    {
        description = "원점으로 돌아갑니다.";
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
        agent.SetDestination(agent.MoveRangeCenterPos);
        return ENodeState.Success;
    }
}
