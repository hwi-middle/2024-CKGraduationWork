using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyAiData _aiData;
    [SerializeField] private Transform _patrolPointsRoot;

    private float _perceptionGauge = 0f;
    private Vector3 _moveRangeCenterPos;
    public float PerceptionGauge => _perceptionGauge;
    private readonly Collider[] _overlappedPlayerBuffer = new Collider[1];
    private Transform _foundPlayer;
    private NavMeshAgent _navMeshAgent;
    private IEnumerator _patrolRoutine;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        var perceptionNote = Instantiate(Resources.Load("PerceptionNote/PerceptionNote"), FindObjectOfType<Canvas>().transform).GetComponent<PerceptionNote>();
        perceptionNote.owner = this;
        _moveRangeCenterPos = transform.position;
    }

    private void Start()
    {
        _navMeshAgent.speed = _aiData.moveSpeed;
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, _moveRangeCenterPos) > _aiData.moveRange)
        {
            _navMeshAgent.SetDestination(_moveRangeCenterPos);
        }
        else
        {
            DetectPlayer();
        }
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
        if (Vector3.Dot(direction, transform.forward) < Mathf.Cos(_aiData.perceptionAngle * 0.5f * Mathf.Deg2Rad))
        {
            _foundPlayer = null;
            return;
        }

        // 시야에 플레이어가 들어옴
        _foundPlayer = overlappedPlayer;
        float distance = Vector3.Distance(transform.position, overlappedPlayer.position);
        _perceptionGauge += GetPerceptionGaugeIncrement(distance);
        _perceptionGauge = Mathf.Clamp(_perceptionGauge, 0, 100);
        if (Mathf.Approximately(_perceptionGauge, 100f))
        {
            StopCoroutine(_patrolRoutine);
            _navMeshAgent.SetDestination(overlappedPlayer.position);
        }
    }

    private float GetPerceptionGaugeIncrement(float distanceToPlayer)
    {
        float distanceRatio = Mathf.Clamp(distanceToPlayer / _aiData.perceptionDistance, 0, 1);

        for (int i = 0; i < _aiData.perceptionRanges.Count; i++)
        {
            if (distanceRatio <= _aiData.perceptionRanges[i].rangePercent / 100)
            {
                return _aiData.perceptionRanges[i].gaugeIncrementPerSecond * Time.deltaTime;
            }
        }

        Debug.Assert(false);
        return -1f;
    }

    private Vector3 CalculateDirectionVector(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.gray;
        for (int i = 0; i < _aiData.perceptionRanges.Count - 1; i++) // 마지막 와이어는 따로 그림
        {
            Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, 360, _aiData.perceptionRanges[i].rangePercent * _aiData.perceptionDistance / 100);
        }

        Handles.DrawWireArc(
            transform.position,
            Vector3.up,
            transform.rotation * CalculateDirectionVector(_aiData.perceptionAngle / 2),
            360 - _aiData.perceptionAngle,
            _aiData.perceptionDistance);

        Handles.color = _foundPlayer == null ? Color.white : Color.red;
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, -_aiData.perceptionAngle / 2, _aiData.perceptionDistance, 2.0f);
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, _aiData.perceptionAngle / 2, _aiData.perceptionDistance, 2.0f);
        Handles.DrawLine(transform.position,
            transform.position + transform.rotation * CalculateDirectionVector(-_aiData.perceptionAngle / 2) * _aiData.perceptionDistance, 2.0f);
        Handles.DrawLine(transform.position,
            transform.position + transform.rotation * CalculateDirectionVector(_aiData.perceptionAngle / 2) * _aiData.perceptionDistance, 2.0f);

        if (_foundPlayer != null)
        {
            Handles.DrawLine(transform.position, _foundPlayer.position, 2.0f);
        }

        Handles.color = Color.green;
        Handles.DrawWireArc(Application.isPlaying ? _moveRangeCenterPos : transform.position, Vector3.up, transform.forward, 360, _aiData.moveRange);
    }
#endif

    private void Patrol()
    {
        _patrolRoutine = PatrolRoutine();
        StartCoroutine(_patrolRoutine);
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            for (int i = 0; i < _patrolPointsRoot.childCount; i++)
            {
                Vector3 targetPos = _patrolPointsRoot.GetChild(i).position;
                _navMeshAgent.SetDestination(targetPos);

                float remainingDistance = Vector3.Distance(transform.position, targetPos);
                while (remainingDistance > _navMeshAgent.stoppingDistance)
                {
                    remainingDistance = _navMeshAgent.pathPending ? Vector3.Distance(transform.position, targetPos) : _navMeshAgent.remainingDistance;
                    yield return null;
                }
            }
        }
    }
}
