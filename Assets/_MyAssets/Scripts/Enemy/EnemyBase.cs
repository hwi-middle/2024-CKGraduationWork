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

    [SerializeField] private CenterSightBound _centerSight;
    public SightBound CenterSight => _centerSight;
    [SerializeField] private SideSightBound _sideSight;
    public SightBound SideSight => _sideSight;
    
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

    private float GetPerceptionGaugeIncrement()
    {
        float distanceRatio = _centerSight.GetPlayerPositionRatio();
        float multiplier = _aiData.perceptionGaugeCurve.Evaluate(distanceRatio);
        float increment = _aiData.maxPerceptionGaugeIncrementPerSecond * multiplier * Time.deltaTime;
        Debug.Assert(increment >= 0);
        return increment;
    }

    public bool IsArrivedToTarget(Vector3 target)
    {
        float remainingDistance = _navMeshAgent.pathPending ? Vector3.Distance(transform.position, target) : _navMeshAgent.remainingDistance;
        return remainingDistance <= _navMeshAgent.stoppingDistance;
    }

    public void IncrementPerceptionGauge()
    {
        _perceptionGauge += GetPerceptionGaugeIncrement();
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
