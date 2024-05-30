using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonTextColorChanger : MonoBehaviour
{
    private TMP_Text _text;
    public Color defaultColor;
    public Color changeColor;
    
    public void ChangeButtonTextColorToHighlighted(Button button)
    {
        _text = button.transform.GetComponentInChildren<TMP_Text>();
        _text.color = changeColor;
        AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.UI_Select);
    }

    public void ChangeButtonTextColorToDefault(Button button)
    {
        _text = button.transform.GetComponentInChildren<TMP_Text>();
        _text.color = defaultColor;
    }
}
