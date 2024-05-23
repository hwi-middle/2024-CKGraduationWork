using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinateZone : MonoBehaviour
{
    [SerializeField] private PlayerInputData _playerInputData;
    [SerializeField] private EnemyBase _targetEnemy;
    [SerializeField] private GameObject _assassinateUI;
    [SerializeField] private Transform _cameraPoint;
    private bool _isInZone;



    private void OnEnable()
    {
        _playerInputData.assassinateEvent += HandleAssassinateAction;
    }

    private void OnDisable()
    {
        _playerInputData.assassinateEvent -= HandleAssassinateAction;
    }

    private void HandleAssassinateAction()
    {
        if (!_isInZone)
        {
            return;
        }
        
        // Todo : 암살 애니메이션 출력, 카메라 전환, 암살 액션 종료 시 오브젝트 파괴
        CameraController.Instance.ChangeCameraToAssassinate(_cameraPoint, transform);
        Destroy(_targetEnemy.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        _isInZone = true;
        // _assassinateUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        _isInZone = false;
        // _assassinateUI.SetActive(false);
    }
}
