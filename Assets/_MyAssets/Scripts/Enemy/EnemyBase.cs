using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public enum EPerceptionPhase
    {
        None,
        Suspicion,
        Alert,
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

    [SerializeField] private Animator _animator;

    [FormerlySerializedAs("_isHearingDisabled")] [SerializeField] private bool _isHearingItemSoundDisabled;
    
    public bool IsDead { get; set; }

    //private Animator _animator;

    // "AK" stands for Animator Key
    private static readonly int AK_Speed = Animator.StringToHash("Speed");
    private static readonly int AK_IsDead = Animator.StringToHash("IsDead");

    private float _perceptionGauge = 0f;
    private Vector3 _moveRangeCenterPos;
    public Vector3 MoveRangeCenterPos => _moveRangeCenterPos;
    private Transform _foundPlayer;
    private NavMeshAgent _navMeshAgent;
    private IEnumerator _patrolRoutine;
    private PerceptionNote _perceptionNote;

    public float PerceptionGauge => _perceptionGauge;
    public bool IsPerceptionGaugeFull => _perceptionGauge >= 100f;

    private bool IsBtPaused = false;
    public PlayerMove PlayerMoveInstance { get; private set; }
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    [SerializeField] private Transform _rayOrigin;
    public Transform RayOrigin => _rayOrigin;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _perceptionNote = Instantiate(Resources.Load("PerceptionNote/PerceptionNote"), _canvas.transform).GetComponent<PerceptionNote>();
        _perceptionNote.owner = this;
        _moveRangeCenterPos = transform.position;
        //_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _navMeshAgent.stoppingDistance = _aiData.stoppingDistance;
        _navMeshAgent.speed = _aiData.walkSpeed;
        _tree = _tree.Clone();
        _tree.Bind(this);
        PlayerMoveInstance = PlayerMove.Instance;
    }

    void Update()
    {
        if (!IsBtPaused)
        {
            _tree.Update();
        }

        _animator.SetFloat(AK_Speed, _navMeshAgent.velocity.magnitude);
        _animator.SetBool(AK_IsDead, IsDead);
        if (_perceptionGauge > 0 && !PlayerMoveInstance.IsAssassinating)
        {
            SSPerceptionGaugeUiHandler.Instance.RegisterEnemy(this);
            SSPerceptionGaugeUiHandler.Instance.UpdateEnemyPerceptionGauge(this);
        }
        else
        {
            SSPerceptionGaugeUiHandler.Instance.UnregisterEnemy(this);
        }

        // 에디터가 아니라면 프레임마다 할당해 줄 필요는 없음
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

    public void OnListenItemSound(Vector3 origin, float increment)
    {
        if (_isHearingItemSoundDisabled)
        {
            return;
        }
        
        _tree.blackboard.lastTimeNoiseDetected = Time.time;
        _perceptionGauge = Mathf.Clamp(_perceptionGauge, _aiData.alertThreshold, 100f);

        StartCoroutine(FollowItemSoundRoutine(origin));
    }

    private IEnumerator FollowItemSoundRoutine(Vector3 target)
    {
        IsBtPaused = true;
        SetDestination(target);
        while (!IsArrivedToTarget(target))
        {
            yield return null;
        }

        IsBtPaused = false;
    }

    public void OnListenNoiseSound(Vector3 origin, float increment)
    {
        _tree.blackboard.lastTimeNoiseDetected = Time.time;
        
        // 청각만으로는 Detection 단계까지 올라가지 않음
        if (_perceptionGauge >= _aiData.maxPerceptionGaugeByHearing)
        {
            return;
        }
        _perceptionGauge = Mathf.Clamp(_perceptionGauge + increment, 0f, _aiData.maxPerceptionGaugeByHearing);
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

    public int TakeDamage(int damageAmount, GameObject damageCauser)
    {
        Debug.Assert(damageCauser == Player.Instance.gameObject);
        Destroy(gameObject);
        return damageAmount;
    }

    public EPerceptionPhase GetCurrentPerceptionPhase()
    {
        Debug.Assert(_aiData.alertThreshold is > 0f and < 100f);

        if (_perceptionGauge >= 100f)
        {
            return EPerceptionPhase.Detection;
        }

        if (_perceptionGauge >= _aiData.alertThreshold)
        {
            return EPerceptionPhase.Alert;
        }

        if (_perceptionGauge > Mathf.Epsilon)
        {
            return EPerceptionPhase.Suspicion;
        }

        return EPerceptionPhase.None;
    }
}
