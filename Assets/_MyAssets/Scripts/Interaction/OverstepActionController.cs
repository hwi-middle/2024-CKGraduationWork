using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverstepActionController : Singleton<OverstepActionController>
{
    [SerializeField] private PlayerData _data;
    private IEnumerator _overstepActionRoutine;
    
    private Vector3 _startPosition;
    private Vector3 _firstTargetPosition;
    private Vector3 _secondTargetPosition;

    private float _yTopPoint;
    
    public void OverstepAction(Transform targetTransform, float distance)
    {
        if (_overstepActionRoutine != null)
        {
            return;
        }

        Transform playerTransform = transform;
        Ray ray = new Ray(playerTransform.position, playerTransform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)
            || hit.transform != targetTransform)
        {
            return;
        }

        Vector3 hitTransformPosition = hit.transform.position;
        hitTransformPosition.y = hit.point.y;
        Vector3 direction = -hit.normal;
        Collider hitCollider = hit.transform.GetComponent<Collider>();
        _yTopPoint = hitCollider.bounds.max.y;

        _firstTargetPosition = distance * 0.5f * direction;
        _firstTargetPosition += hitTransformPosition;
        _firstTargetPosition.y += _yTopPoint;

        _secondTargetPosition = distance * direction;
        _secondTargetPosition += hitTransformPosition;
        
        _overstepActionRoutine = OverstepActionRoutine();
        StartCoroutine(_overstepActionRoutine);
    }
    
    private IEnumerator OverstepActionRoutine()
    {
        float t = 0;
        _startPosition = transform.position;
        PlayerMove.Instance.AddPlayerState(EPlayerState.Overstep);
        
        // 최고점으로 이동
        while (t <= _data.overstepActionDuration / 2)
        {
            // 시간 절반 이전
            float alpha = t / (_data.overstepActionDuration * 0.5f);
            transform.position =
                Vector3.Lerp(_startPosition, _firstTargetPosition, alpha);
            yield return null;
            t += Time.deltaTime;
        }

        // 최고점에서 바닥으로 이동
        while (t <= _data.overstepActionDuration)
        {
            // 시간 절반 이후
            float alpha = (t - _data.overstepActionDuration * 0.5f) / (_data.overstepActionDuration * 0.5f);
            transform.position =
                Vector3.Lerp(_firstTargetPosition, _secondTargetPosition, alpha);
            yield return null;
            t += Time.deltaTime;
        }
        
        PlayerMove.Instance.RemovePlayerState(EPlayerState.Overstep);
        _overstepActionRoutine = null;
    }
}
