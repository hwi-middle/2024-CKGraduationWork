using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPerceptionGaugeZero : DecoratorNode
{
    public override void OnCreate()
    {
        description = "인지 게이지가 0인지 확인합니다.";
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
        if (agent.PerceptionGauge > 0.0f)
        {
            return ENodeState.Failure;
        }
        
        SSPerceptionGaugeUiHandler.Instance.UnregisterEnemy(agent);

        switch (child.Update())
        {
            case ENodeState.InProgress:
                return ENodeState.InProgress;
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
