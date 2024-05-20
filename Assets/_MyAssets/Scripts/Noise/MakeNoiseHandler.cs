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
    private bool _isOnMakeNoise;
#endif
    private const int MAX_ENEMY_COUNT = 10;
    private readonly Collider[] _enemiesBuffer = new Collider[MAX_ENEMY_COUNT];

    private void Awake()
    {
#if UNITY_EDITOR
        StartCoroutine(ResetOnMakeNoiseFlagRoutine());
#endif
    }

    public void OnMakeNoise(float impactRadius, float increment)
    {
#if UNITY_EDITOR
        _impactRadius = impactRadius;
        _isOnMakeNoise = true;
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
    private IEnumerator ResetOnMakeNoiseFlagRoutine()
    {
        var waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            yield return waitForEndOfFrame;
            _isOnMakeNoise = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!SceneManagerBase.Instance.IsDebugMode)
        {
            return;
        }

        if (!_isOnMakeNoise)
        {
            return;
        }

        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, _impactRadius, 0.1f);
    }
#endif
}
