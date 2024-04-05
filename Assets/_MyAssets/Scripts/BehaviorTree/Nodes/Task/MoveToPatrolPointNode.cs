using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPatrolPointNode : TaskNode
{
    public override void OnCreate()
    {
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

        return ENodeState.Running;
    }
}
