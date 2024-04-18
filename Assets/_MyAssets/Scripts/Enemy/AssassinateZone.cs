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
        GameObject enemy = _targetEnemy.gameObject;
        if (!_isInZone)
        {
            return;
        }
        
        Debug.Log($"Target Enemy : {_targetEnemy.name} Called");

        int key = _targetEnemy.transform.GetInstanceID();
        MiddleSaveData.Instance.KillEnemy(key);
        _targetEnemy.gameObject.SetActive(false);
        _isInZone = false;
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
