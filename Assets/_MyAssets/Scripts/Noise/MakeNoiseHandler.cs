using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MakeNoiseHandler : MonoBehaviour
{

    public float impactRadius;

    private readonly Collider[] _enemiesBuffer = new Collider[10];

    public void OnMakeNoise(float increment)
    {
        // Overlap Sphere 로 적 감지되면 적 감지 로직 실행
        int layerMask = LayerMask.GetMask("Enemy");
        int size = Physics.OverlapSphereNonAlloc(transform.position, impactRadius, _enemiesBuffer, layerMask);
        if (size == 0)
        {
            return;
        }

        foreach (Collider enemy in _enemiesBuffer)
        {
            enemy.gameObject.GetComponent<EnemyBase>().OnListenNoiseSound(transform.position, increment);
        }
    }
}
