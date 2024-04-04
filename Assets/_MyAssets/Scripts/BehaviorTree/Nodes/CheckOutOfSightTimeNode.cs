using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOutOfSightTimeNode : DecoratorNode
{
    public float threshold = 1.0f;

    public override void OnCreate()
    {
        description = "시야에서 사라진 후 일정 시간이 지났는지 확인합니다.";
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
        if (blackboard.outOfSightTime >= threshold)
        {
            child.Update();
            return ENodeState.Success;
        }

        return ENodeState.Failure;
    }
}
