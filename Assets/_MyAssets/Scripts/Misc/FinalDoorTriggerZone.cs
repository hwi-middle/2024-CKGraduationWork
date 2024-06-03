using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoorTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManagerBase.Instance.GetComponent<IngameSceneManager>().LoadCreditScene();
        }
    }
}
