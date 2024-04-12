
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : CompositeNode
{
    public override void OnCreate()
    {
        description = "자신의 자식들을 순차적으로 실행합니다.";
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

        for (int index = currentChildIndex; index < children.Count; index++)
        {
            Node child = children[index];
            switch (child.Update())
            {
                case ENodeState.InProgress:
                    currentChildIndex = index;
                    return ENodeState.InProgress;
                case ENodeState.Failure:
                    return ENodeState.Failure;
                case ENodeState.Aborted:
                    return ENodeState.Aborted;
            }
        }

        return ENodeState.Success;
    }
}
