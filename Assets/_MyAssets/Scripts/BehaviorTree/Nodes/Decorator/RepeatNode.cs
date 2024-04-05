using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatNode : DecoratorNode
{
    public override void OnCreate()
    {
        description = "자신의 자식을 반복해서 실행합니다.";
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
        child.Update();
        return ENodeState.Running;
    }
}
