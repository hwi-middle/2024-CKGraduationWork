
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    private int current;

    public override void OnCreate()
    {
        description = "자신의 자식들 중 하나를 실행합니다.";
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
        
        Node child = children[current];
        switch (child.Update())
        {
            case ENodeState.Running:
                return ENodeState.Running;
            case ENodeState.Failure:
                break;
            case ENodeState.Success:
                current++;
                break;
            case ENodeState.Aborted:
                return ENodeState.Aborted;
            default:
                Debug.Assert(false);
                break;
        }
        
        return current == children.Count ? ENodeState.Success : ENodeState.Running;
    }
}
