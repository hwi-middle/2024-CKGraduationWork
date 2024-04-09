using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCanAtackNode : DecoratorNode
{
    public override void OnCreate()
    {
        description = "공격 가능한지 확인합니다.";
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
        Debug.Log("CheckCanAttackNode OnUpdate()");
        if (blackboard.target == null)
        {
            return ENodeState.Failure;
        }
        
        if (Vector3.Distance(agent.transform.position, blackboard.target.transform.position) <= agent.AiData.attackRange)
        {
            child.Update();
            return ENodeState.Success;
        }

        return ENodeState.Failure;

    }
}
