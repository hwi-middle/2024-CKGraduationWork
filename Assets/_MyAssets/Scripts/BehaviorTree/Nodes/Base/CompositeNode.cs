using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> children = new List<Node>();
    
    private int _lastFrameCount = -1;
    protected int currentChildIndex = 0;
    
    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.name = node.name.Replace("(Clone)", "");
        node.children = new List<Node>();
        foreach (Node child in children)
        {
            node.children.Add(child.Clone());
        }
        return node;
    }
    
    protected bool IsCalledLastFrame()
    {
        bool result = _lastFrameCount == Time.frameCount - 1;
        _lastFrameCount = Time.frameCount;
        
        return result;
    }
}
