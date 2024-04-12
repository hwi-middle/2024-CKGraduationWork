using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    public override void OnCreate()
    {
        description = "자신의 자식들 중 하나를 실행합니다.";
    }

    protected override void OnStart()
    {
        if (!IsCalledLastFrame())
        {
            currentChildIndex = 0;
        }
    }

    protected override void OnStop()
    {
        currentChildIndex = 0;
    }

    protected override void OnAbort()
    {
        currentChildIndex = 0;
    }

    protected override ENodeState OnUpdate()
    {
        if (children.Count == 0)
        {
            return ENodeState.Success;
        }

        foreach (var child in children)
        {
            switch (child.Update())
            {
                case ENodeState.InProgress:
                    return ENodeState.InProgress;
                case ENodeState.Success:
                    return ENodeState.Success;
            }
        }

        return ENodeState.Failure;
    }
}
