using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPerceptionGaugeFullNode : DecoratorNode
{
    public override void OnCreate()
    {
        description = "인지 게이지가 가득 찼는지 확인합니다.";
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
        if (!agent.IsPerceptionGaugeFull)
        {
            return ENodeState.Failure;
        }
        
        switch (child.Update())
        {
            case ENodeState.Running:
                return ENodeState.Running;
            case ENodeState.Failure:
                return ENodeState.Failure;
            case ENodeState.Success:
                return ENodeState.Success;
            case ENodeState.Aborted:
                return ENodeState.Aborted;
            default:
                Debug.Assert(false);
                return ENodeState.Failure;
        }
    }
}
