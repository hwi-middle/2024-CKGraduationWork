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
        tree.Bind(GetComponent<EnemyBase>());
    }

    private void Update()
    {
        tree.Update();
    }
}
