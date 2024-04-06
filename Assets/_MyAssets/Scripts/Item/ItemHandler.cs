using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    private float _gaugeIncreaseAmount;
    private float _impactRadius;

    public void Init(float gaugeAmount, float impactRadius)
    {
        _gaugeIncreaseAmount = gaugeAmount;
        _impactRadius = impactRadius;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        // Overlap Sphere 로 적 감지되면 적 감지 로직 실행
        LayerMask layerMask = LayerMask.GetMask("Enemy");
        Collider[] enemies = Physics.OverlapSphere(transform.position, _impactRadius, layerMask);
        if (enemies.Length != 0)
        {
            foreach (Collider enemy in enemies)
            {
                enemy.gameObject.GetComponent<EnemyBase>().OnListenStrangeSound(transform.position, _gaugeIncreaseAmount);
            }
        }
        
        Destroy(gameObject);
    }
}
