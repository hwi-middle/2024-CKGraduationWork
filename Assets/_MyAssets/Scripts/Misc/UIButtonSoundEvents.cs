using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSoundEvents : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons = new();

    private void Awake()
    {
        Debug.Assert(_buttons.Count != 0, "Not Exist Buttons");
        InitButtonsEventTrigger();
    }

    private void InitButtonsEventTrigger()
    {
        foreach (Button button in _buttons)
        {
            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
            
            // Enter Event
            EventTrigger.Entry pointEnterEntry = new()
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointEnterEntry.callback.AddListener((x) => { OnEnterMousePoint();});
            eventTrigger.triggers.Add(pointEnterEntry);
            
            // Click Event
            EventTrigger.Entry pointerClickEntry = new()
            {
                eventID = EventTriggerType.PointerClick
            };
            pointerClickEntry.callback.AddListener((x) => { OnClickButton();});
            eventTrigger.triggers.Add(pointerClickEntry);
        }
    }

    private void OnEnterMousePoint()
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.UI_Select);
    }
    
    private void OnClickButton()
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.UI_Click);
    }
}
