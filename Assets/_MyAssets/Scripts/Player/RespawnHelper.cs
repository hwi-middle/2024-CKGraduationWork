using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHelper : Singleton<RespawnHelper>
{
    [SerializeField] private Transform _enemies;
    [SerializeField] private Transform _items;
    
    // Dead Animation 출력 시간 임시 조치
    [SerializeField] private float _deadRoutineDuration;
    
    [SerializeField] private GameObject _playerModel;

    public Vector3 LastCheckPoint { get; private set; }
    
    public GameObject PlayerModel => _playerModel;

    public Transform Enemies => _enemies;
    public Transform Items => _items;

    private IEnumerator _respawnRoutine;

    private void Awake()
    {
        LastCheckPoint = transform.position;
    }

    private void Update()
    {
        CheckPlayerHp();
    }

    private void RespawnPlayer()
    {
        if (_respawnRoutine != null)
        {
            return;
        }

        _respawnRoutine = RespawnRoutine();
        StartCoroutine(_respawnRoutine);
    }

    private IEnumerator RespawnRoutine()
    {
        PlayerMove.Instance.SetDeadState();
        yield return new WaitForSeconds(_deadRoutineDuration);
        _playerModel.SetActive(false);
        Player.Instance.Respawn();
        _respawnRoutine = null;
    }

    private void CheckPlayerHp()
    {
        if (Player.Instance.Hp > 0)
        {
            return;
        }
        
        RespawnPlayer();
    }

    public void SaveCheckPoint(Vector3 checkPoint)
    {
        LastCheckPoint = checkPoint;
    }
}
