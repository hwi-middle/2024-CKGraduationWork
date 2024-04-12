using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : TaskNode
{
    public override void OnCreate()
    {
        description = "플레이어에게 데미지를 입힙니다.";
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
        if (blackboard.target == null)
        {
            return ENodeState.Failure;
        }

        Debug.Log("AttackPlayer");
        blackboard.target.GetComponent<Player>().TakeDamage(agent.AiData.attackDamage, agent.gameObject);
        return ENodeState.Success;
    }
}
