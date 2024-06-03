using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseNode : TaskNode
{
    public override void OnCreate()
    {
        description = "플레이어를 추적합니다.";
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
        agent.NavMeshAgent.isStopped = false;
        if (blackboard.target == null)
        {
            return ENodeState.Failure;
        }
        
        agent.SetSpeed(agent.AiData.sprintSpeed);
        agent.SetDestination(blackboard.target.transform.position);
        
        return ENodeState.Success;
    }
}