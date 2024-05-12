using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class MakeNoiseHandler : MonoBehaviour
{
#if UNITY_EDITOR
    private float _impactRadius; // for gizmo
#endif
    private const int MAX_ENEMY_COUNT = 10;
    private readonly Collider[] _enemiesBuffer = new Collider[MAX_ENEMY_COUNT];

    public void OnMakeNoise(float impactRadius, float increment)
    {
#if UNITY_EDITOR
        _impactRadius = impactRadius;
#endif
        // Overlap Sphere 로 적 감지되면 적 감지 로직 실행
        int layerMask = LayerMask.GetMask("Enemy");
        int size = Physics.OverlapSphereNonAlloc(transform.position, impactRadius, _enemiesBuffer, layerMask);
        if (size == 0)
        {
            return;
        }

        for (int index = 0; index < size; index++)
        {
            Collider enemy = _enemiesBuffer[index];
            enemy.gameObject.GetComponent<EnemyBase>().OnListenNoiseSound(transform.position, increment);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!SceneManagerBase.Instance.IsDebugMode)
        {
            return;
        }
        
        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, _impactRadius, 0.1f);
    }
#endif
}
