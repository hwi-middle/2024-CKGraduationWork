using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlyHandler : MonoBehaviour
{
    private float _gaugeIncreaseAmount;
    private float _impactRadius;
    private readonly Collider[] _enemiesBuffer = new Collider[10];

    public void Init(float gaugeAmount, float impactRadius)
    {
        _gaugeIncreaseAmount = gaugeAmount;
        _impactRadius = impactRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("InteractionTriggerZone"))
        {
            return;
        }

        // Overlap Sphere 로 적 감지되면 적 감지 로직 실행
        int layerMask = LayerMask.GetMask("Enemy");
        int size = Physics.OverlapSphereNonAlloc(transform.position, _impactRadius, _enemiesBuffer, layerMask);
        if (size != 0)
        {
            foreach (Collider enemy in _enemiesBuffer)
            {
                enemy.gameObject.GetComponent<EnemyBase>().OnListenItemSound(transform.position, _gaugeIncreaseAmount);
            }
        }

        Destroy(gameObject);
    }
}
