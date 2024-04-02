using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogNode : ActionNode
{
    public string message;

    public override void OnCreate()
    {
        description = "디버그용 로그를 출력합니다.";
    }

    protected override void OnStart()
    {
       Debug.Log($"OnStart{message}");
    }

    protected override void OnStop()
    {
        Debug.Log($"OnStop{message}");
    }

    protected override void OnAbort()
    {
        
    }

    protected override ENodeState OnUpdate()
    {
        Debug.Log($"OnUpdate{message}");
        return ENodeState.Success;
    }
}
