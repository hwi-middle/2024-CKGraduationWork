using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectTriggerZone : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _animator.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        _animator.enabled = true;
    }
}
