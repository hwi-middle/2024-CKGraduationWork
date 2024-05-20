using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerZoneController : MonoBehaviour
{
    public ETutorialVideoIndex tutorialVideoIndex;
    
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = SceneManagerBase.Instance.IsDebugMode;
        Debug.Assert(tutorialVideoIndex != ETutorialVideoIndex.None, "Tutorial Video Index is None");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PopupHandler.Instance.DisplayTutorialPopup("Tutorial", "Tutorial Message",
            "OK", tutorialVideoIndex, HandleTutorialPopupButtonAction);
    }


    private void HandleTutorialPopupButtonAction(bool isPositive)
    {
        Destroy(gameObject);       
    }
}
