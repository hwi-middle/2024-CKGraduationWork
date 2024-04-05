using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOutOfRangeNode : DecoratorNode
{
    public override void OnCreate()
    {
        description = "자신의 활동 범위를 벗어났는지 확인합니다.";
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
        if (Vector3.Distance(agent.transform.position, agent.MoveRangeCenterPos) > agent.AiData.moveRange)
        {
            child.Update();
            return ENodeState.Success;
        }

        return ENodeState.Failure;
    }
}
