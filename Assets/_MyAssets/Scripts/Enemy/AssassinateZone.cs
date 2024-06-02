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
    [SerializeField] private Transform _assassinateOffset;
    private bool _isInZone;

    private EnemyBase _parent;


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
        if (!_isInZone || PlayerMove.Instance.IsAssassinating)
        {
            return;
        }
        
        // 카메라 전환 후 애니메이션이 끝날 때 까지 대기
        CameraController.Instance.ChangeCameraToAssassinate(_cameraPoint, transform.parent);
        PlayerMove.Instance.AssassinateEnemy(_assassinateOffset);
        StartCoroutine(AwaitAssassinateEndRoutine());
    }
    
    private IEnumerator AwaitAssassinateEndRoutine()
    {
        // 애니메이션이 끝날 때 까지 대기
        while (PlayerMove.Instance.IsAssassinating)
        {
            yield return null;
        }
        
        Destroy(transform.parent.gameObject);
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
