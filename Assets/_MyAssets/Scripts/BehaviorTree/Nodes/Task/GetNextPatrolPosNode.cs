using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNextPatrolPosNode : TaskNode
{
    public enum EGetPositionMethod
    {
        Sequential,
        Random,
    }

    public EGetPositionMethod getPositionMethod = EGetPositionMethod.Sequential;

    private int _prevIdx = -1;
    private int _currentIdx = 0;

    public override void OnCreate()
    {
        description = "다음 순찰 지점을 얻습니다.";
    }

    protected override void OnStart()
    {
        for (int idx = 0; idx < agent.transform.childCount; idx++)
        {
            Transform child = agent.transform.GetChild(idx);
            if (child.CompareTag("PatrolPoint"))
            {
                blackboard.patrolPoints.Add(agent.transform.GetChild(idx).position);
            }
        }
    }

    protected override void OnStop()
    {
    }

    protected override void OnAbort()
    {
    }

    protected override ENodeState OnUpdate()
    {
        if (blackboard.patrolPoints.Count == 0)
        {
            return ENodeState.Failure;
        }

        switch (getPositionMethod)
        {
            case EGetPositionMethod.Sequential:
                _currentIdx++;
                if (_currentIdx >= blackboard.patrolPoints.Count)
                {
                    _currentIdx = 0;
                }
                break;
            case EGetPositionMethod.Random:
                do
                {
                    _currentIdx = Random.Range(0, blackboard.patrolPoints.Count);
                } while (_currentIdx == _prevIdx);
                break;
            default:
                Debug.Assert(false);
                return ENodeState.Failure;
        }

        blackboard.nextPatrolPos = blackboard.patrolPoints[_currentIdx];

        return ENodeState.Success;
    }
}
