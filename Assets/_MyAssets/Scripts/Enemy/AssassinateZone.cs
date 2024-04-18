using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinateZone : MonoBehaviour
{
    [SerializeField] private PlayerInputData _playerInputData;
    [SerializeField] private EnemyBase _targetEnemy;
    [SerializeField] private GameObject _assassinateUI;
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
        
        MiddleSaveData.Instance.KillEnemy(_targetEnemy.gameObject);
        _targetEnemy.gameObject.SetActive(false);
        //Destroy(_targetEnemy.gameObject);
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
