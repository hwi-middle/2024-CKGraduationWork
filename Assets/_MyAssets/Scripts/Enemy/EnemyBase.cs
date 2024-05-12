using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public enum EPerceptionPhase
    {
        None,
        Perceive,
        Suspect,
        Detection,
    }
    
    [SerializeField] private EnemyAiData _aiData;
    public EnemyAiData AiData => _aiData;
    [SerializeField] private BehaviorTree _tree;
    public BehaviorTree Tree => _tree;
    [SerializeField] private Canvas _canvas;

    [SerializeField] private PerceptionBound _centerSight;
    public PerceptionBound CenterSight => _centerSight;
    [SerializeField] private PerceptionBound _sideSight;
    public PerceptionBound SideSight => _sideSight;
    
    private Animator _animator;

    // "AK" stands for Animator Key
    private static readonly int AK_Speed = Animator.StringToHash("Speed");

    private float _perceptionGauge = 0f;
    private Vector3 _moveRangeCenterPos;
    public Vector3 MoveRangeCenterPos => _moveRangeCenterPos;
    private Transform _foundPlayer;
    private NavMeshAgent _navMeshAgent;
    private IEnumerator _patrolRoutine;
    private PerceptionNote _perceptionNote;

    public float PerceptionGauge => _perceptionGauge;
    public bool IsPerceptionGaugeFull => _perceptionGauge >= 100f;
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _perceptionNote = Instantiate(Resources.Load("PerceptionNote/PerceptionNote"), _canvas.transform).GetComponent<PerceptionNote>();
        _perceptionNote.owner = this;
        _moveRangeCenterPos = transform.position;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _navMeshAgent.stoppingDistance = _aiData.stoppingDistance;
        _navMeshAgent.speed = _aiData.walkSpeed;
        _tree = _tree.Clone();
        _tree.Bind(this);
    }

    void Update()
    {
        _tree.Update();
        _animator.SetFloat(AK_Speed, _navMeshAgent.velocity.magnitude);
        
#if UNITY_EDITOR
        _navMeshAgent.stoppingDistance = _aiData.stoppingDistance;
#endif
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

    public void OnListenItemSound(Vector3 origin, float increase)
    {
        _perceptionGauge = Mathf.Clamp(_perceptionGauge, 50f, 100f);
    }

    public void SetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }

    public void SetSpeed(float speed)
    {
        _navMeshAgent.speed = speed;
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
// #if UNITY_EDITOR
//    private Vector3 CalculateDirectionVector(float angle)
//    {
//        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
//    }
//     private void OnDrawGizmos()
//     {
//         Handles.color = Color.gray;
//         for (int i = 0; i < _aiData.perceptionRanges.Count - 1; i++) // 마지막 와이어는 따로 그림
//         {
//             Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, 360,
//                 _aiData.perceptionRanges[i].rangePercent * _aiData.perceptionDistance / 100);
//         }
//
//         Handles.DrawWireArc(
//             transform.position,
//             Vector3.up,
//             transform.rotation * CalculateDirectionVector(_aiData.perceptionAngle / 2),
//             360 - _aiData.perceptionAngle,
//             _aiData.perceptionDistance);
//
//         Handles.color = _foundPlayer == null ? Color.white : Color.red;
//         Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, -_aiData.perceptionAngle / 2, _aiData.perceptionDistance, 2.0f);
//         Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, _aiData.perceptionAngle / 2, _aiData.perceptionDistance, 2.0f);
//         Handles.DrawLine(transform.position,
//             transform.position + transform.rotation * CalculateDirectionVector(-_aiData.perceptionAngle / 2) * _aiData.perceptionDistance, 2.0f);
//         Handles.DrawLine(transform.position,
//             transform.position + transform.rotation * CalculateDirectionVector(_aiData.perceptionAngle / 2) * _aiData.perceptionDistance, 2.0f);
//
//         if (_foundPlayer != null)
//         {
//             Handles.DrawLine(transform.position, _foundPlayer.position, 2.0f);
//         }
//
//         Handles.color = Color.green;
//         Handles.DrawWireArc(Application.isPlaying ? _moveRangeCenterPos : transform.position, Vector3.up, transform.forward, 360, _aiData.moveRange);
//     }
// #endif

    public bool IsArrivedToTarget(Vector3 target)
    {
        float remainingDistance = _navMeshAgent.pathPending ? Vector3.Distance(transform.position, target) : _navMeshAgent.remainingDistance;
        return remainingDistance <= _navMeshAgent.stoppingDistance;
    }

    public void IncrementPerceptionGauge(float distance)
    {
        _perceptionGauge += GetPerceptionGaugeIncrement(distance);
        _perceptionGauge = Mathf.Clamp(_perceptionGauge, 0, 100);
    }

    public void DecrementPerceptionGauge()
    {
        _perceptionGauge -= _aiData.gaugeDecrementPerSecond * Time.deltaTime;
        _perceptionGauge = Mathf.Clamp(_perceptionGauge, 0, 100);
    }

    public void Attack(Player player)
    {
        player.TakeDamage(_aiData.attackDamage, gameObject);
    }

    public int TakeDamage(int damageAmount, GameObject damageCauser)
    {
        Destroy(gameObject);
        return damageAmount;
    }

    public EPerceptionPhase GetCurrentPerceptionPhase()
    {
        if (_perceptionGauge >= 100f)
        {
            return EPerceptionPhase.Detection;
        }

        if (_perceptionGauge >= 50f)
        {
            return EPerceptionPhase.Suspect;
        }

        if (_perceptionGauge > Mathf.Epsilon)
        {
            return EPerceptionPhase.Perceive;
        }

        return EPerceptionPhase.None;
    }
}
