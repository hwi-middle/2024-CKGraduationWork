using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerZoneController : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = SceneManagerBase.Instance.IsDebugMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PopupHandler.Instance.DisplayTutorialPopup(HandleTutorialPopupButtonAction, "Tutorial", "Tutorial Message",
            "OK", ETutorialVideoIndex.Basic);
    }


    private void HandleTutorialPopupButtonAction(bool isPositive)
    {
        Destroy(gameObject);       
    }
}
