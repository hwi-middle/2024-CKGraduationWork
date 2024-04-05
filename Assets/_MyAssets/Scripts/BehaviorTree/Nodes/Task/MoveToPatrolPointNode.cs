using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPatrolPointNode : TaskNode
{
    public override void OnCreate()
    {
        description = "순찰 지점을 순회하며 이동합니다.";
    }

    protected override void OnStart()
    {
        agent.SetDestination(blackboard.nextPatrolPos);
    }

    protected override void OnStop()
    {
    }

    protected override void OnAbort()
    {
    }

    protected override ENodeState OnUpdate()
    {
        if (agent.IsArrivedToTarget(blackboard.nextPatrolPos))
        {
            return ENodeState.Success;
        }

        return ENodeState.InProgress;
    }
}
