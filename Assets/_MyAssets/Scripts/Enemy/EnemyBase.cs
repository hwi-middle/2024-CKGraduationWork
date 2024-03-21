using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private float _perceptionDistance;
    [SerializeField, Range(0, 360)] private float _perceptionAngle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3 GetAngle(float angle) // degree
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(
            transform.position, 
            Vector3.up, 
            GetAngle(_perceptionAngle / 2), 
            360 - _perceptionAngle, 
            _perceptionDistance);
        
        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, -_perceptionAngle / 2, _perceptionDistance, 2.0f);
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, _perceptionAngle / 2, _perceptionDistance, 2.0f);
        Handles.DrawLine(transform.position, transform.position + GetAngle(-_perceptionAngle / 2) * _perceptionDistance, 2.0f);
        Handles.DrawLine(transform.position, transform.position + GetAngle(_perceptionAngle / 2) * _perceptionDistance, 2.0f);
    }
}
