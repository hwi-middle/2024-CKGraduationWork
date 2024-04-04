using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerOnSight : DecoratorNode
{
    private readonly Collider[] _overlappedPlayerBuffer = new Collider[1];

    public override void OnCreate()
    {
        description = "플레이어가 시야에 들어왔는지 확인합니다.";
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
        int bufferCount = Physics.OverlapSphereNonAlloc(agent.transform.position, agent.AiData.perceptionDistance, _overlappedPlayerBuffer, LayerMask.GetMask("Player"));
        Debug.Assert(bufferCount is 0 or 1);
        if (bufferCount == 0)
        {
            blackboard.target = null;
            return ENodeState.Failure;
        }

        Transform overlappedPlayer = _overlappedPlayerBuffer[0].transform;
        Debug.Assert(overlappedPlayer != null);

        Vector3 direction = (overlappedPlayer.position - agent.transform.position).normalized;
        if (Vector3.Dot(direction, agent.transform.forward) < Mathf.Cos(agent.AiData.perceptionAngle * 0.5f * Mathf.Deg2Rad))
        {
            blackboard.target = null;
            return ENodeState.Failure;
        }

        // 나(AI)와 플레이어 사이에 장애물이 있는지 확인
        Vector3 rayDirection = (overlappedPlayer.position - agent.transform.position).normalized;
        Ray ray = new Ray(agent.transform.position, rayDirection);

        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);
        if (!hit.transform.CompareTag("Player"))
        {
            blackboard.target = null;
            return ENodeState.Failure;
        }

        // 시야에 플레이어가 들어옴
        blackboard.target = overlappedPlayer.gameObject;
        return ENodeState.Success;
    }
}
