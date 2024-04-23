using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloatingObject : MonoBehaviour
{
    private float _elapsedTime = 0f;
    private float _originalY;
    
    [SerializeField] private bool _isRandomSpeed;
    [SerializeField] private float _moveSpeed = 1;
    [SerializeField] private float _minMoveSpeed;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private bool _isRandomAmount;
    [SerializeField] private float _moveAmount = 1;
    [SerializeField] private float _minMoveAmount;
    [SerializeField] private float _maxMoveAmount;

    private void Awake()
    {
        _originalY = transform.position.y;
    }

    void Start()
    {
        if (_isRandomSpeed)
        {
            _moveSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
        }
        
        if (_isRandomAmount)
        {
            _moveAmount = Random.Range(_minMoveAmount, _maxMoveAmount);
        }
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, _originalY + Mathf.Sin(_elapsedTime * _moveSpeed) * _moveAmount, transform.position.z);
    }
}
