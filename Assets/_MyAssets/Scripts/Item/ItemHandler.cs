using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        // Overlap Sphere 로 적 감지되면 적 감지 로직 실행
        
        Destroy(gameObject);
    }
}
