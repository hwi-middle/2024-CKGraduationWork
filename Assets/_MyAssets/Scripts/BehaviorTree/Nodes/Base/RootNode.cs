using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    [HideInInspector] public Node child;

    public override void OnCreate()
    {
        description = "행동트리의 시작지점입니다.";
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
        return child.Update();
    }
    
    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        node.name = node.name.Replace("(Clone)", "");
        return node;
    }
}
