using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    private void Start()
    {
        tree = tree.Clone();
        // TODO: Bind the tree to the agent
        // tree.Bind(GetComponent<AiAgent>());
    }

    private void Update()
    {
        tree.Update();
    }
}
