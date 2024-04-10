using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPatrolPointNode : TaskNode
{
    public enum EGetPositionMethod
    {
        Sequential,
        Random,
    }

    public EGetPositionMethod getPositionMethod = EGetPositionMethod.Sequential;

    private int _prevIdx = -1;
    private int _currentIdx = -1;
    private List<Vector3> _patrolPoints = new List<Vector3>();
    private Vector3 _nextPatrolPos;

    public override void OnCreate()
    {
        description = "순찰 지점을 순회하며 이동합니다.";
    }

    protected override void OnStart()
    {
        agent.SetSpeed(agent.AiData.walkSpeed);
        GetNextPatrolPos();
        agent.SetDestination(_nextPatrolPos);
    }

    protected override void OnStop()
    {
    }

    protected override void OnAbort()
    {
    }

    protected override ENodeState OnUpdate()
    {
        if (agent.IsArrivedToTarget(_nextPatrolPos))
        {
            return ENodeState.Success;
        }

        return ENodeState.InProgress;
    }

    private void GetNextPatrolPos()
    {
        for (int idx = 0; idx < agent.transform.childCount; idx++)
        {
            Transform child = agent.transform.GetChild(idx);
            if (child.CompareTag("PatrolPoint"))
            {
                _patrolPoints.Add(agent.transform.GetChild(idx).position);
            }
        }

        switch (getPositionMethod)
        {
            case EGetPositionMethod.Sequential:
                _currentIdx++;
                if (_currentIdx >= _patrolPoints.Count)
                {
                    _currentIdx = 0;
                }

                break;
            case EGetPositionMethod.Random:
                do
                {
                    _currentIdx = Random.Range(0, _patrolPoints.Count);
                } while (_currentIdx == _prevIdx);

                break;
            default:
                Debug.Assert(false);
                break;
        }
        
        _nextPatrolPos = _patrolPoints[_currentIdx];
        _prevIdx = _currentIdx;
    }
}
