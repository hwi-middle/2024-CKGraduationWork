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
        Debug.Log("추적 시작");
    }

    protected override void OnStop()
    {
        Debug.Log("추적 중지");
    }

    protected override void OnAbort()
    {
        
    }

    protected override ENodeState OnUpdate()
    {
        Debug.Log("추적 중");
        
        
        
        return ENodeState.Success;
    }
}