
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
    }

    protected override void OnStop()
    {
        
    }

    protected override void OnAbort()
    {
        
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
                case ENodeState.Failure:
                    return ENodeState.Failure;
                case ENodeState.Aborted:
                    return ENodeState.Aborted;
            }
        }

        return ENodeState.Success;
    }
}
