using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableZoneHandler : MonoBehaviour
{
    public static int hideableZoneCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }
        
        hideableZoneCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }
        
        Debug.Assert(hideableZoneCount != 0, "hideableZoneCount != 0");
        
        hideableZoneCount--;
    }
}
