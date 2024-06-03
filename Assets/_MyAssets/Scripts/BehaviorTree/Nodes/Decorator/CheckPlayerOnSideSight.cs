using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: CenterSight와 SideSight의 처리가 매우 유사하여 묶어야함(상속 등)
public class CheckPlayerOnSideSight : DecoratorNode
{
    public override void OnCreate()
    {
        description = "플레이어가 사이드 시야에 들어왔는지 확인합니다.";
    }

    protected override void OnStart()
    {
        // SSPerceptionGaugeUiHandler.Instance.RegisterEnemy(agent);
    }

    protected override void OnStop()
    {
        // SSPerceptionGaugeUiHandler.Instance.UpdateEnemyPerceptionGauge(agent);
    }

    protected override void OnAbort()
    {
    }

    protected override ENodeState OnUpdate()
    {
        if (!IsPlayerOnSight())
        {
            return ENodeState.Failure;
        }
        
        child.Update();
        return ENodeState.Success;
    }

    private bool IsPlayerOnSight()
    {
        // 시야 범위 내에 들었는지 확인
        if (!agent.SideSight.IsOnSight)
        {
            blackboard.target = null;
            return false;
        }

        // 나(AI)와 플레이어 사이에 장애물이 있는지 확인
        Player player = Player.Instance;
        Debug.Assert(player != null);
        Transform playerTransform = player.transform;
        Vector3 rayDirection = (playerTransform.position - agent.RayOrigin.position).normalized;
        // var ray = new Ray(agent.transform.position, rayDirection);
        var ray = new Ray(agent.RayOrigin.position, rayDirection);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) || !hit.transform.CompareTag("Player"))
        {
            blackboard.target = null;
            return false;
        }
        
        // 시야에 플레이어가 들어옴
        blackboard.target = playerTransform.gameObject;
        blackboard.lastTimePlayerDetected = Time.time;
        return true;
    }
}
