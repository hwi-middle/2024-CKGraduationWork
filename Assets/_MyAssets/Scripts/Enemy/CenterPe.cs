using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterSightBound : SightBound
{
    private Player _player;
    private Transform _sightStart;
    private Transform _sightEnd;

    private void Awake()
    {
        _player = Player.Instance;
        Debug.Assert(_player != null);
        Debug.Assert(transform.childCount >= 2);
        _sightStart = transform.GetChild(0);
        _sightEnd = transform.GetChild(1);
    }

    public float GetPlayerPositionRatio()
    {
        // 플레이어가 Z축을 기준으로 _sightOrigin과 _sightEnd 사이에 있는 비율을 반환
        // _sightOrigin과 가까울 수록 0, _sightEnd와 가까울 수록 1
        
        // _sightOrigin과 _sightEnd 사이의 거리를 구함
        float totalZDistance = Mathf.Abs(_sightStart.position.z - _sightEnd.position.z);

        // _sightOrigin과 플레이어 사이의 거리를 구함
        float playerZDistance = Mathf.Abs(_sightStart.position.z - _player.transform.position.z);

        // 두 거리의 비율을 계산하여 반환
        return Mathf.Clamp(playerZDistance / totalZDistance, 0f, 1f);
    }
}
