using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLastHearingTimeNode : DecoratorNode
{
    public float thresholdOnSuspicion;
    public float thresholdOnAlert;

    public override void OnCreate()
    {
        description = "소리를 들은 후 일정 시간이 지났는지 확인합니다.";
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
        float threshold = -1f;

        switch (agent.GetCurrentPerceptionPhase())
        {
            case EnemyBase.EPerceptionPhase.Suspicion:
                threshold = thresholdOnSuspicion;
                break;
            case EnemyBase.EPerceptionPhase.Alert:
                threshold = thresholdOnAlert;
                break;
            case EnemyBase.EPerceptionPhase.None:
            case EnemyBase.EPerceptionPhase.Detection:
                return ENodeState.Success;
            default:
                Debug.Assert(false);
                break;
        }
        
        if (Time.time - blackboard.lastTimeNoiseDetected >= threshold)
        {
            child.Update();
            return ENodeState.Success;
        }

        return ENodeState.Failure;
    }
}
