
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : CompositeNode
{
    private int current;

    public override void OnCreate()
    {
        description = "자신의 자식들을 순차적으로 실행합니다.";
    }
    
    protected override void OnStart()
    {
        current = 0;
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
        
        var child = children[current];
        switch (child.Update())
        {
            case ENodeState.Running:
                return ENodeState.Running;
            case ENodeState.Failure:
                return ENodeState.Failure;
            case ENodeState.Success:
                current++;
                break;
            default:
                Debug.Assert(false);
                break;
        }
        
        return current == children.Count ? ENodeState.Success : ENodeState.Running;
    }
}
