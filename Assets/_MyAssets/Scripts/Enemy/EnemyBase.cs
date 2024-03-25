using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyAiData _aiData;
    [SerializeField] private Transform _patrolPointsRoot;
    [SerializeField] private Canvas _canvas;

    private float _perceptionGauge = 0f;
    private Vector3 _moveRangeCenterPos;
    public float PerceptionGauge => _perceptionGauge;
    private readonly Collider[] _overlappedPlayerBuffer = new Collider[1];
    private Transform _foundPlayer;
    private NavMeshAgent _navMeshAgent;
    private IEnumerator _patrolRoutine;
    private PerceptionNote _perceptionNote;
    private float _timeAfterPlayerOutOfSight = 0f;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _perceptionNote = Instantiate(Resources.Load("PerceptionNote/PerceptionNote"), _canvas.transform).GetComponent<PerceptionNote>();
        _perceptionNote.owner = this;
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

    private void OnDestroy()
    {
        if (_patrolRoutine != null)
        {
            StopCoroutine(_patrolRoutine);
            _patrolRoutine = null;
        }

        if (_perceptionNote != null)
        {
            Destroy(_perceptionNote.gameObject);
        }
    }

    private void DetectPlayer()
    {
        if (IsPlayerOnSight(out _foundPlayer))
        {
            Debug.Assert(_foundPlayer != null);
            _timeAfterPlayerOutOfSight = 0f;
            float distance = Vector3.Distance(transform.position, _foundPlayer.position);
            _perceptionGauge += GetPerceptionGaugeIncrement(distance);
            _perceptionGauge = Mathf.Clamp(_perceptionGauge, 0, 100);
            if (Mathf.Approximately(_perceptionGauge, 100f))
            {
                if (_patrolRoutine != null)
                {
                    StopCoroutine(_patrolRoutine);
                    _patrolRoutine = null;
                }
                _navMeshAgent.SetDestination(_foundPlayer.position);
            }
        }
        else if (_perceptionGauge > 0f)
        {
            _timeAfterPlayerOutOfSight += Time.deltaTime;
            if (_timeAfterPlayerOutOfSight >= _aiData.gaugeDecreaseStartTime)
            {
                _perceptionGauge -= _aiData.gaugeDecrementPerSecond * Time.deltaTime;
                _perceptionGauge = Mathf.Clamp(_perceptionGauge, 0, 100);
            }
            
            if (_patrolRoutine == null && Mathf.Approximately(_perceptionGauge, 0f))
            {
                Patrol();
            }
        }
    }

    private bool IsPlayerOnSight(out Transform result)
    {
        int bufferCount = Physics.OverlapSphereNonAlloc(transform.position, _aiData.perceptionDistance, _overlappedPlayerBuffer, LayerMask.GetMask("Player"));
        Debug.Assert(bufferCount is 0 or 1);
        if (bufferCount == 0)
        {
            result = null;
            return false;
        }
        
        Transform overlappedPlayer = _overlappedPlayerBuffer[0].transform;
        Debug.Assert(overlappedPlayer != null);

        Vector3 direction = (overlappedPlayer.position - transform.position).normalized;
        if (Vector3.Dot(direction, transform.forward) < Mathf.Cos(_aiData.perceptionAngle * 0.5f * Mathf.Deg2Rad))
        {
            result = null;
            return false;
        }

        // 나(AI)와 플레이어 사이에 장애물이 있는지 확인
        Vector3 rayDirection = (overlappedPlayer.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, rayDirection);

        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);
        if (!hit.transform.CompareTag("Player"))
        {
            result = null;
            return false;
        }

        // 시야에 플레이어가 들어옴
        result = overlappedPlayer;
        return true;
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
            Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, 360,
                _aiData.perceptionRanges[i].rangePercent * _aiData.perceptionDistance / 100);
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
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}
