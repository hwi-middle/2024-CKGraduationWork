using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class Blackboard
{
    // public Vector3 moveToPosition;
    public GameObject target;
    public float lastTimePlayerDetected;
    public readonly List<Vector3> patrolPoints = new List<Vector3>();
    public Vector3 nextPatrolPos;
}
