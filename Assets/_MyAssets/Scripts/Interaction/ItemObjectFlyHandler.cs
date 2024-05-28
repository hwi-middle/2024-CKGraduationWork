using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectFlyHandler : MonoBehaviour
{
    private float _gaugeIncreaseAmount;
    private float _impactRadius;
    private readonly Collider[] _enemiesBuffer = new Collider[10];
    private static readonly int AK_IS_PLAYING = Animator.StringToHash("IsPlaying");
    private const string ANIMATION_STATE_NAME = "Play";
    private Animator _animator;
    private bool _isCollided = false;

    public void Init(float gaugeAmount, float impactRadius)
    {
        _animator = GetComponentInChildren<Animator>();
        _gaugeIncreaseAmount = gaugeAmount;
        _impactRadius = impactRadius;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("InteractionTriggerZone") || _isCollided)
        {
            return;
        }

        // Overlap Sphere 로 적 감지되면 적 감지 로직 실행
        _isCollided = true;
        _animator.SetBool(AK_IS_PLAYING, true);
        StartCoroutine(AwaitAnimationPlayingAndDestroyRoutine());
        
        int layerMask = LayerMask.GetMask("Enemy");
        int size = Physics.OverlapSphereNonAlloc(transform.position, _impactRadius, _enemiesBuffer, layerMask);
        if (size != 0)
        {
            for (int index = 0; index < size; index++)
            {
                Collider enemy = _enemiesBuffer[index];
                enemy.gameObject.GetComponent<EnemyBase>().OnListenItemSound(transform.position, _gaugeIncreaseAmount);
            }
        }
    }

    private IEnumerator AwaitAnimationPlayingAndDestroyRoutine()
    {
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATION_STATE_NAME))
        {
            yield return null;
        }
        
        Debug.Assert(_animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATION_STATE_NAME), "Animation State Name Error");
        
        float t = 0;
        while (t < 1)
        {
            yield return null;
            t = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
        
        Destroy(gameObject);
    }
}
