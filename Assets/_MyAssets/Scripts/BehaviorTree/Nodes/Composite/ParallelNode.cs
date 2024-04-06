using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelNode : CompositeNode
{
    public override void OnCreate()
    {
        description = "자신의 자식들을 모두 순차 실행합니다.";
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
            child.Update();
        }

        return ENodeState.Success;
    }
}
