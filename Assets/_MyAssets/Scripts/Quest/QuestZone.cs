using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestZone : MonoBehaviour
{
    [HideInInspector] public Quest quest;
    [SerializeField] private Sprite _questImage;
    private bool _isActivated = false;

    private void Awake()
    {
        GetComponent<Renderer>().enabled = SceneManagerBase.Instance.IsDebugMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivated)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        QuestHandler.Instance.ShowQuestText(quest.description);
        QuestHandler.Instance.ShowQuestImage(_questImage);
        _isActivated = true;
    }
}
