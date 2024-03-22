using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyAiData _aiData;
    private readonly Collider[] _overlappedPlayerBuffer = new Collider[1];
    private Transform _foundPlayer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        int bufferCount = Physics.OverlapSphereNonAlloc(transform.position, _aiData.perceptionDistance, _overlappedPlayerBuffer, LayerMask.GetMask("Player"));
        Debug.Assert(bufferCount is 0 or 1);
        if (bufferCount == 0)
        {
            _foundPlayer = null;
            return;
        }
        Transform overlappedPlayer = _overlappedPlayerBuffer[0].transform;
        Debug.Assert(overlappedPlayer != null);

        Vector3 direction = (overlappedPlayer.position - transform.position).normalized;
        if (Vector3.Dot(direction, transform.forward) < Mathf.Cos( _aiData.perceptionAngle * 0.5f * Mathf.Deg2Rad))
        {
            _foundPlayer = null;
            return;
        }

        _foundPlayer = overlappedPlayer;
    }

    private Vector3 CalculateDirectionVector(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.gray;
        Handles.DrawWireArc(
            transform.position,
            Vector3.up,
            CalculateDirectionVector(_aiData.perceptionAngle / 2),
            360 - _aiData.perceptionAngle,
            _aiData.perceptionDistance);

        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, -_aiData.perceptionAngle / 2, _aiData.perceptionDistance, 2.0f);
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, _aiData.perceptionAngle / 2, _aiData.perceptionDistance, 2.0f);
        Handles.DrawLine(transform.position, transform.position + CalculateDirectionVector(-_aiData.perceptionAngle / 2) * _aiData.perceptionDistance, 2.0f);
        Handles.DrawLine(transform.position, transform.position + CalculateDirectionVector(_aiData.perceptionAngle / 2) * _aiData.perceptionDistance, 2.0f);

        if (_foundPlayer != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, _foundPlayer.position, 2.0f);
        }
    }
}
